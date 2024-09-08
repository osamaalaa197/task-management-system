using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Core;
using TaskManagement.Core.Consts;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Models;
using TaskManagement.EF;
using TaskManagement.Extensions;
using TaskManagement.Filter;
using TaskManagement.Hubs;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtector _dataProtector;
        private readonly IFileService _fileService;
        public IHubContext<TaskCountHub> _taskCountHub;
        public IHubContext<SendMessageHub> _sendMessage;

        public AssignmentController(IMapper mapper, IUnitOfWork unitOfWork, IDataProtectionProvider dataProtector, IFileService fileService,IHubContext<TaskCountHub> taskCountHub,IHubContext<SendMessageHub> sendMessage)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _dataProtector = dataProtector.CreateProtector("TaskManagement");
            _fileService = fileService;
            _taskCountHub = taskCountHub;
            _sendMessage = sendMessage;
        }
        [Authorize]
        public IActionResult Index()
        {
            var assignmentList = new List<Assignment>();
            var role = User.Identities.FirstOrDefault()?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            switch (role)
            {
                case Roles.Administrator:
                    assignmentList = _unitOfWork.Assignments.FindAllWithInclude(e => !e.IsDeleted, e => e.AssignedToUser).ToList();
                    break;
                case Roles.TeamLeader:
                    var userId = User.GetUserId();
                    var teamId = _unitOfWork.Users.GetById(userId).TeamId;
                    assignmentList = _unitOfWork.Assignments.FindAllWithInclude(e => !e.IsDeleted && e.TeamId == teamId, e => e.AssignedToUser).ToList();
                    break;
                case Roles.User:
                    var userID = User.GetUserId();
                    assignmentList = _unitOfWork.Assignments.FindAllWithInclude(e => !e.IsDeleted && e.AssignedToUserId == userID, e => e.AssignedToUser).ToList();
                    break;
                default:
                    break;
            }
            var listAssignment = assignmentList
                .Select(assignment =>
                {
                    var viewModel = _mapper.Map<AssignmentViewModel>(assignment);
                    viewModel.Key = _dataProtector.Protect(viewModel.Key);
                    viewModel.AssignedUser = assignment.AssignedToUser is not null ? assignment.AssignedToUser.FullName : null;
                    return viewModel;
                })
                .ToList();

            return View(listAssignment);
        }
        [Authorize]
        public IActionResult CreateAssignment()
        {
            var res = new AssignmentViewModel();
            res.AssignmentStatusList = Enum.GetValues(typeof(AssignmentStatus))
                 .Cast<AssignmentStatus>()
                 .Select(pl => new SelectListItem
                 {
                     Value = pl.ToString(),
                     Text = pl.ToString()
                 });
            res.PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                 .Cast<PriorityLevel>()
                 .Select(pl => new SelectListItem
                 {
                     Value = pl.ToString(),
                     Text = pl.ToString()
                 });
            res.DueDate = DateTime.Now;
            return View(res);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SaveAssignment(AssignmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AssignmentStatusList = Enum.GetValues(typeof(AssignmentStatus))
                 .Cast<AssignmentStatus>()
                 .Select(pl => new SelectListItem
                 {
                     Value = pl.ToString(),
                     Text = pl.ToString()
                 });
                model.PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                     .Cast<PriorityLevel>()
                     .Select(pl => new SelectListItem
                     {
                         Value = pl.ToString(),
                         Text = pl.ToString()
                     });
                return View("CreateAssignment", model);
            }
            var role = User.Identities.FirstOrDefault()?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var assignment = _mapper.Map<Assignment>(model);
            assignment.CreatedById = User.GetUserId();
            assignment.CreatedOn = DateTime.Now;
            assignment.IsDeleted = false;
            assignment.AssignedToUserId = role == Roles.User ? User.GetUserId() : null;
            _unitOfWork.Assignments.Add(assignment);
            _unitOfWork.Complete();
            if (model.AttachmentFile is not null)
            {
                if (!_fileService.AllowFile(model.AttachmentFile))
                    return BadRequest(Errors.MaxSize);
                if (!await _fileService.AllowFileExtension(model.AttachmentFile))
                    return BadRequest(Errors.NotAllowedContent);
                var path = _fileService.UploadFile(assignment.Id, model.AttachmentFile);
                var attachment = new Attachments()
                {
                    CreatedById = User.GetUserId(),
                    CreatedOn = DateTime.Now,
                    AssignmentId = assignment.Id,
                    FilePath = path
                };
                _unitOfWork.Attachments.Add(attachment);
                _unitOfWork.Complete();
            }
            var user = _unitOfWork.Users.GetById(assignment.CreatedById);
            var auditTrail = new AuditTrail()
            {
                ActionType = ActionType.TaskCreated.ToString(),
                ActionDescription = $"Task '{assignment.Title}' was created by {user.FullName}",
                UserId =user.Id,
                UserName=user.FullName,
                TaskId=assignment.Id,
                Timestamp=DateTime.Now
            };
            _unitOfWork.AuditTrails.Add(auditTrail);
            _unitOfWork.Complete();
            TempData["success"] = Messages.TaskAdded;
            return RedirectToAction("Index");
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetAssignment(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return BadRequest();
            }
            //Handel Comment and Team Assign
            var assignmentId = _dataProtector.Unprotect(id);
            var currentAssignment = _unitOfWork.Assignments.FindWithInclude(e => e.Id == int.Parse(assignmentId), e => e.Attachments, e => e.Comments, e => e.Team,e=>e.AssignedToUser);
            if (currentAssignment == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<AssignmentViewModel>(currentAssignment);
            if (currentAssignment.Comments.Any())
                model.Comment = currentAssignment.Comments.Select(e => new CommentViewModel() { Comment = e.comment }).ToList();
            if (currentAssignment.AssignedToUser is not null)
                model.AssignedUser = currentAssignment.AssignedToUser.FullName;
            if (currentAssignment.TeamId is not null)
                model.AssignedTeam = currentAssignment.Team.TeamName;
            return View("AssignmentDetail", model);
        }
        [Authorize(Roles = Roles.TeamLeader + "," + Roles.Administrator)]
        public async Task<IActionResult> AssignAssignment(string acid)
        {
            var currentUserId = User.GetUserId();
            var currentRole = GetCurrentUserRole();
            var model = new AssignFormViewModel
            {
                AssignmentId = _dataProtector.Protect(acid)
            };

            if (currentRole == Roles.TeamLeader)
            {
                PopulateTeamMembers(currentUserId, model);
            }
            else if (currentRole == Roles.Administrator)
            {
                await PopulateAdminData(model);
            }
            return View("AssignTask", model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignAssignment(AssignFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var currentUserId = User.GetUserId();
                var currentRole = GetCurrentUserRole();
                if (currentRole == Roles.TeamLeader)
                {
                    PopulateTeamMembers(currentUserId, model);
                }
                else if (currentRole == Roles.Administrator)
                {
                    await PopulateAdminData(model);
                }
                return View("AssignTask", model);
            }
            var assignmentId = _dataProtector.Unprotect(model.AssignmentId);
            var currentAssignment = _unitOfWork.Assignments.Find(e => e.Id == int.Parse(assignmentId));
            if (currentAssignment == null)
            {
                return NotFound();
            }
            if (model.IsAssignedToTeam)
            {
                await AssignToTeam(currentAssignment, model);
            }
            else
            {
                await AssignToUser(currentAssignment, model);
            }
            var user = _unitOfWork.Users.GetById(currentAssignment.CreatedById);
            var auditTrail = new AuditTrail()
            {
                ActionType = ActionType.AssignmentUpdated.ToString(),
                ActionDescription = $"Task '{currentAssignment.Title}' was Assigned by {user.FullName}",
                UserId = user.Id,
                UserName = user.FullName,
                TaskId = currentAssignment.Id,
                Timestamp = DateTime.Now
            };
            _unitOfWork.AuditTrails.Add(auditTrail);
            _unitOfWork.Complete();
            TempData["success"] = Messages.TaskAssign;
            return RedirectToAction("Index");
        }
        [Authorize]
        public async Task<IActionResult> UpdateStatus(string acid)
        {
            var model = new UpdateStatusViewModel();
            model.StatusList = Enum.GetValues(typeof(AssignmentStatus))
                 .Cast<AssignmentStatus>()
                 .Select(pl => new SelectListItem
                 {
                     Value = pl.ToString(),
                     Text = pl.ToString()
                 }).ToList();
            model.AssignmentId = _dataProtector.Protect(acid);
            return View("UpdateTask", model);
        }
        [HttpPost]
        public IActionResult UpdateStatus(UpdateStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UpdateTask",new { acid=_dataProtector.Unprotect(model.AssignmentId)});
            }
            var assignmentId = _dataProtector.Unprotect(model.AssignmentId);
            var currentAssignment = _unitOfWork.Assignments.Find(e => e.Id == int.Parse(assignmentId));
            if (currentAssignment == null)
                return NotFound();
            currentAssignment.Status = model.NewStatus;
            _unitOfWork.Complete();
            var user = _unitOfWork.Users.GetById(currentAssignment.CreatedById);
            var auditTrail = new AuditTrail()
            {
                ActionType = ActionType.StatusChanged.ToString(),
                ActionDescription = $"Task '{currentAssignment.Title}' was change status by {user.FullName}",
                UserId = user.Id,
                UserName = user.FullName,
                TaskId = currentAssignment.Id,
                Timestamp = DateTime.Now
            };
            _unitOfWork.AuditTrails.Add(auditTrail);
            _unitOfWork.Complete();
            TempData["success"] = Messages.UpdateStatus;
            return RedirectToAction("GetAssignment", new { id = model.AssignmentId } );
        }
        [Authorize(Roles = Roles.TeamLeader + "," + Roles.Administrator)]
        public IActionResult GetAssignmentByMemberId(string memberId)
        {
            var task=_unitOfWork.Assignments.FindAll(e=>e.AssignedToUserId == memberId).ToList();
            var data = task.Select(e => new AssignmentViewModel
            {
                Title = e.Title,
                Description = e.Description,
                DueDate = e.DueDate,
                PriorityLevel = e.PriorityLevel,
                Status = e.Status
            }).ToList();
            return View("AssignmentForMember", data);
        }

        private string GetCurrentUserRole()
        {
            return User.Identities.FirstOrDefault()?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }
        private void PopulateTeamMembers(string userId, AssignFormViewModel model)
        {
            var team = _unitOfWork.Users.GetById(userId).Team;
            model.ListUser = team.Members.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.FullName
            }).ToList();
        }
        private async Task PopulateAdminData(AssignFormViewModel model)
        {
            var usersInRole = await _unitOfWork.Users.GetUserByRole(Roles.User.ToString(), true);
            model.ListUser = usersInRole.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.FullName
            }).ToList();

            model.ListTeam = _unitOfWork.Teams.GetAll()
                .Select(e => new SelectListItem
                {
                    Value = e.TeamId.ToString(),
                    Text = e.TeamName
                }).ToList();
        }
        private async Task AssignToUser(Assignment assignment, AssignFormViewModel model)
        {
            var user = _unitOfWork.Users.GetById(model.UserId);
            assignment.AssignedToUserId = model.UserId;
            _unitOfWork.Complete();
            var countTask = _unitOfWork.Assignments.FindAll(e => e.AssignedToUserId == model.UserId).Count();
            await _taskCountHub.Clients.User(model.UserId).SendAsync("ReceiveTaskUpdate", countTask);

            string body = EmailTemplates.GetTaskAssignmentBody(user.FullName, assignment.Title, assignment.Description, assignment.DueDate);
            MailService.SendMessage(user.Email, "Assigned Task", body);
        }
        private async Task AssignToTeam(Assignment assignment, AssignFormViewModel model)
        {
            assignment.TeamId = model.TeamId;
            if (model.MemberId == null)
            {
                var re = _unitOfWork.Teams.FindWithInclude(e => e.TeamId == model.TeamId,e=>e.Members,f=>f.Assignments);
                var teamMembersId = re.Members.Select(e => e.Id).ToList();
                await _sendMessage.Clients.Users(teamMembersId).SendAsync("SendMessage", Messages.TaskAssignToTeam);
                await _taskCountHub.Clients.Users(teamMembersId).SendAsync("ReceiveTaskUpdate", re.Assignments.Count());
            }
            else
            {
                assignment.AssignedToUserId = model.MemberId;
                MailService.SendMessage(model.MemberId, "Assigned Task", body);
                await _sendMessage.Clients.User(model.MemberId).SendAsync("SendMessage", Messages.TaskAssignToTeam);
            }
            _unitOfWork.Complete();
        }


    }
}
