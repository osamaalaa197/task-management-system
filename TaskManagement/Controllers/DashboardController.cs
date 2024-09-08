using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagement.Core;
using TaskManagement.Core.Consts;
using TaskManagement.Core.Enums;
using TaskManagement.EF;
using TaskManagement.Extensions;
using TaskManagement.Models;
using TaskManagement.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        [Authorize]
        public IActionResult Index()
        {
            var role = User.Identities.FirstOrDefault()?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role==Roles.Administrator)
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (role==Roles.TeamLeader)
            {
                return RedirectToAction("TeamLeaderDashboard");
            }
            else
            {
                return RedirectToAction("UserDashboard");
            }
        }

        #region UserDashboard
        [Authorize]
        public IActionResult UserDashboard()
        {
            var userId = User.GetUserId();
            var countTask = _unitOfWork.Assignments.FindAll(e => e.AssignedToUserId == userId).Count();
            return View(countTask);
        }
        public IActionResult GetAssignedTask()
        {
            var userId = User.GetUserId();
            var data = _unitOfWork.Assignments.FindAll(e => e.AssignedToUserId == userId);
            var taskInProgress = data.Where(e => e.Status == AssignmentStatus.InPogress.ToString()).Count();
            var taskNotStarted = data.Where(e => e.Status == AssignmentStatus.NotStarted.ToString()).Count();
            var taskComplete = data.Where(e => e.Status == AssignmentStatus.Complete.ToString()).Count();
            var model = new AssignedTaskViewModel() { TaskCompleted=taskComplete,TaskInProgress=taskInProgress,TaskNotStarted=taskNotStarted};
            return Json(new {Data= model });
        }
        public IActionResult GetCompletedTask()
        {
            var userId = User.GetUserId();
            var task = _unitOfWork.Assignments.FindAll(e => e.AssignedToUserId == userId);
            var totalTask = task.Count();
            var completedTasks = task.Where(e=>e.Status == AssignmentStatus.Complete.ToString()).Count();
            return Json(new {TotalTask=totalTask,CompletedTasks=completedTasks});
        }
        public IActionResult GetTaskByPriorityLevel()
        {
            var userId = User.GetUserId();
            var data = _unitOfWork.Assignments.FindAll(e => e.AssignedToUserId == userId);
            var taskLow = data.Where(e => e.PriorityLevel == PriorityLevel.Low.ToString()).Count();
            var taskMedium = data.Where(e => e.PriorityLevel == PriorityLevel.Medium.ToString()).Count();
            var taskHigh = data.Where(e => e.PriorityLevel == PriorityLevel.High.ToString()).Count();
            return Json(new { TaskLow=taskLow,TaskMedium=taskMedium,TaskHigh=taskHigh });
        }
        public IActionResult GetAssignments()
        {
            var userId = User.GetUserId();
            var data= _unitOfWork.Assignments.FindAll(e=>e.AssignedToUserId==userId);
            return Json(data);
        }
        #endregion


        #region TeamLeader
        [Authorize(Roles = Roles.TeamLeader)]

        public IActionResult TeamLeaderDashboard()
        {
            var userId = User.GetUserId();
            var temaId = _unitOfWork.Users.Find(e => e.Id == userId).TeamId;
            var data = _unitOfWork.Assignments.FindAllWithInclude(e => e.TeamId == temaId).AsNoTracking().Count();
            return View(data);
        }

        public IActionResult GetAssignedTaskForTeamByStatus()
        {
            var userId = User.GetUserId();
            var temaId = _unitOfWork.Users.Find(e => e.Id == userId).TeamId;
            var data = _unitOfWork.Assignments.FindAllWithInclude(e => e.TeamId == temaId).AsNoTracking();
            var taskInProgress = data.Where(e => e.Status == AssignmentStatus.InPogress.ToString()).AsNoTracking();
            var taskNotStarted = data.Where(e => e.Status == AssignmentStatus.NotStarted.ToString()).AsNoTracking();
            var taskComplete = data.Where(e => e.Status == AssignmentStatus.Complete.ToString()).AsNoTracking();
            var model = new { TaskCompleted = taskInProgress, TaskInProgress = taskInProgress, TaskNotStarted = taskNotStarted };
            return Json(new { Data = model });
        }
        public IActionResult GetTasksTeamByPriorityLevel()
        {
            var userId = User.GetUserId();
            var temaId = _unitOfWork.Users.Find(e => e.Id == userId).TeamId;
            var data = _unitOfWork.Assignments.FindAllWithInclude(e => e.TeamId == temaId).AsNoTracking();
            var taskLow = data.Where(e => e.PriorityLevel == PriorityLevel.Low.ToString()).Count();
            var taskMedium = data.Where(e => e.PriorityLevel == PriorityLevel.Medium.ToString()).Count();
            var taskHigh = data.Where(e => e.PriorityLevel == PriorityLevel.High.ToString()).Count();
            return Json(new { TaskLow = taskLow, TaskMedium = taskMedium, TaskHigh = taskHigh });
        }

        public IActionResult GetTaskCompletedForMembers()
        {
            var userId = User.GetUserId();
            var temaId = _unitOfWork.Users.Find(e => e.Id == userId).TeamId;
            var teams = _unitOfWork.Teams.FindWithInclude(e => e.TeamId ==temaId, e => e.Members,f=>f.Assignments);
            var memberTaskData = teams.Members.Select(member => new
            {
                MemberName = member.FullName,
                CompletedTaskCount = teams.Assignments.Count(a => a.AssignedToUserId == member.Id && a.Status == AssignmentStatus.Complete.ToString())
            }).ToList();
            return Json(new { Members = memberTaskData });
        }
        #endregion
        public IActionResult GetCompletedTaskForTeam()
        {
            var userId = User.GetUserId();
            var temaId = _unitOfWork.Users.Find(e => e.Id == userId).TeamId;
            var teams = _unitOfWork.Teams.FindWithInclude(e => e.TeamId == temaId, e => e.Members, f => f.Assignments);
            var totalTask = teams.Assignments.Count();
            var completedTasks = teams.Assignments.Where(e => e.Status == AssignmentStatus.Complete.ToString()).Count();
            return Json(new { TotalTask = totalTask, CompletedTasks = completedTasks });
        }
        public IActionResult GetAssignedTaskForTeam()
        {
            var userId = User.GetUserId();
            var teams = _unitOfWork.Teams.FindAllWithInclude(e => e.TeamId != null, e => e.Members).AsNoTracking();
            var teamName = teams.Select(e => e.TeamName).ToList();
            var teamTask = teams.Select(e => e.Assignments.Count());
            return Json(new { TeamName = teamName, TeamTask = teamTask });
        }

        [Authorize(Roles = Roles.Administrator)]
        public IActionResult AdminDashboard()
        {
            var data = _unitOfWork.AuditTrails.FindAllWithInclude(e => e.TaskId != null, e => e.Task).AsNoTracking().ToList();
            return View(data);
        }

    }
}
