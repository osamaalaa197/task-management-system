using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Core;
using TaskManagement.Core.Consts;
using TaskManagement.EF;
using TaskManagement.Extensions;
using TaskManagement.Filter;
using TaskManagement.Models;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{

    public class TeamController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamController(IUnitOfWork unitOfWork,IMapper mapper) 
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        [Authorize]
        public IActionResult Index()
        {
            var currentRole = User.Identities.FirstOrDefault().RoleClaimType.ToString();
            if (currentRole==Roles.TeamLeader)
            {
                var userId = User.GetUserId();
                var data=_unitOfWork.Users.FindAllWithInclude(e=>e.Id==userId,e=>e.Team).Select(e=>new TeamViewModel { Description=e.Team.Description,TeamId=e.TeamId.Value,TeamName=e.Team.TeamName});
                return View(data);
            }
            var teams =_unitOfWork.Teams.GetAll().ToList();
            var model=_mapper.Map<List<TeamViewModel>>(teams);
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles =Roles.Administrator)]
        [AjaxOnly]
        public IActionResult Add()
        {
            var model = new TeamViewModel();
            return PartialView("_AddTeamForm",model);
        }
        [HttpPost]
        public IActionResult Add(TeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddTeamForm", model);
            }
            var team=new Team() { TeamName=model.TeamName,Description=model.Description};
            _unitOfWork.Teams.Add(team);
            _unitOfWork.Complete();
            TempData["success"] = Messages.TeamAdded;
            return RedirectToAction("Index");
        }
        [Authorize(Roles = Roles.TeamLeader + "," + Roles.Administrator)]
        public IActionResult GetMembers(int teamId)
        {
            var team=_unitOfWork.Teams.FindWithInclude(e=>e.TeamId == teamId,e=>e.Members);
            if (team==null)
            {
                return NotFound();
            }
            var model = new TeamMembersViewModel();
            model.TeamName=team.TeamName;
            var res=team.Members.Select(e=>new TeamMembersDataViewModel { MembersName = e.FullName, MemberMobile = e.PhoneNumber ,MemberId=e.Id }).ToList();
            model.TeamId = teamId;
            model.teamMembersViewModels= res;
            return View("TeamMembers", model);
        }
        [Authorize(Roles = Roles.TeamLeader + "," + Roles.Administrator)]
        public IActionResult AddMemberInTeam(int teamId)
        {
            var mode=new AddMembersFormViewModel();
            mode.TeamId = teamId;
            return View("AddMembersForm",mode);
        }
        [Authorize(Roles =Roles.TeamLeader+","+Roles.Administrator)]
        public IActionResult GetMembersById(string memberPhone)
        {
            var data = new TeamMembersDataViewModel();
            if (string.IsNullOrEmpty(memberPhone))
                return Json(new { data = data, Error = Errors.AddMemberPhone });
            var member=_unitOfWork.Users.Find(e=>e.PhoneNumber==memberPhone);
            if (member == null)
                return Json(new { data = data, Error = Errors.MemberPhoneNotFOund });
            data.MemberMobile = member.PhoneNumber;
             data.MemberId = member.Id;
             data.MembersName = member.FullName;
            return Json(new {data=data,Error=""});
        }
        [HttpPost]
        public IActionResult AddMemberInTeam(AddMembersFormViewModel model)
        {
            var team = _unitOfWork.Teams.Find(e=>e.TeamId==model.TeamId);
            foreach (var item in model.teamMembers)
            {
                var member = _unitOfWork.Users.Find(e => e.PhoneNumber == item.MemberMobile);
                team.Members.Add(member);
            }
            _unitOfWork.Complete();
            return RedirectToAction("GetMembers",new { teamId=model.TeamId});
        }
        public IActionResult GetMemberJson(int teamId)
        {
            return Json(_unitOfWork.Teams.FindWithInclude(e => e.TeamId == teamId, e => e.Members).Members.Select(e => new SelectListItem
            {
                Value = e.Id,
                Text = e.FullName

            }));
        }
    }
}
