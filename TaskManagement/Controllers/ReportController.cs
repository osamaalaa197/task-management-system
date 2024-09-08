using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Core;
using TaskManagement.Core.Consts;
using TaskManagement.Core.Enums;
using TaskManagement.Core.Models;
using TaskManagement.Extensions;

namespace TaskManagement.Controllers
{
    public class ReportController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
        }
        [Authorize]
        public IActionResult Index()
        {
            var model = new TaskReportFilters();
            model.TaskStatusList = Enum.GetValues(typeof(AssignmentStatus))
                 .Cast<AssignmentStatus>()
                 .Select(pl => new SelectListItem
                 {
                     Value = pl.ToString(),
                     Text = pl.ToString()
                 });
            model.TaskPriorityLevelList = Enum.GetValues(typeof(PriorityLevel))
                 .Cast<PriorityLevel>()
                 .Select(pl => new SelectListItem
                 {
                     Value = pl.ToString(),
                     Text = pl.ToString()
                 });
            var role = User.Identities.FirstOrDefault()?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            switch (role)
            {
                case Roles.Administrator:
                    model.UsersList= _unitOfWork.Users.GetAll().Select(e=>new SelectListItem {Value=e.Id,Text=e.FullName});
                    model.TeamList = _unitOfWork.Teams.GetAll().Select(e => new SelectListItem { Value = e.TeamId.ToString(), Text = e.TeamName });
                    break;
                case Roles.TeamLeader:
                    var userId = User.GetUserId();
                    var teamId = _unitOfWork.Users.GetById(userId).TeamId;
                    model.UsersList = _unitOfWork.Users.FindAll(e=>e.TeamId==teamId).Select(e => new SelectListItem { Value = e.Id, Text = e.FullName });
                    break;
                default:
                    break;
            }
            return View(model);
        }
        [HttpPost,AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RawData(TaskReportFilters model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var data = GetReportData(model);
            return View(data);
        }
        private IEnumerable<RawData> GetReportData(TaskReportFilters filter)
        {
            var query = _unitOfWork.Assignments.FindAllWithInclude(e => !e.IsDeleted, e => e.AssignedToUser).AsNoTracking();
            
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                query = query.Where(t => t.CreatedOn >= filter.StartDate.Value && t.CreatedOn <= filter.EndDate.Value);
            }
            if (filter.TeamId.HasValue)
            {
                query = query.Where(t => t.TeamId == filter.TeamId.Value);
            }
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                query = query.Where(t => t.AssignedToUserId ==filter.UserId);
            }
            if (!string.IsNullOrEmpty(filter.Status))
            {
                query = query.Where(t => t.Status == filter.Status);
            }
            if (!string.IsNullOrEmpty(filter.TaskPriority))
            {
                query = query.Where(t => t.PriorityLevel == filter.TaskPriority);
            }
            var reportdto=_mapper.Map<List<RawData>>(query.ToList());
            return reportdto;
        }
    }
}
