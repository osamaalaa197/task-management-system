using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagement.Core;
using TaskManagement.Core.Consts;
using TaskManagement.EF;
using TaskManagement.Filter;
using TaskManagement.Models;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUnitOfWork unitOfWork,IMapper mapper,RoleManager<IdentityRole> roleManager) 
        {
            _unitOfWork=unitOfWork;
            _mapper=mapper;
            _roleManager=roleManager;
        }
        [Authorize(Roles = Roles.Administrator)]
        public IActionResult Index()
        {
            var userData = _unitOfWork.Users.FindAllWithInclude(e=>e.EmailConfirmed,e=>e.Team).ToList();
            var data = _mapper.Map<List<UserDataViewModel>>(userData);
            return View(data);
        }
        [Authorize(Roles = Roles.Administrator)]
        [AjaxOnly]
        public IActionResult EditUser(string userId)
        {
            var user = _unitOfWork.Users.FindWithInclude(e => e.Id==userId, e => e.Team);
            if (user==null)
                return NotFound();
           
            var team = _unitOfWork.Teams.GetAll().ToList();
            var model =_mapper.Map<UserDataViewModel>(user);
            model.TeamList = _mapper.Map<List<SelectListItem>>(team);
            return PartialView("_EditUserPartial",model);
        }
        [HttpPost]
        public IActionResult EditUser(UserDataViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user=_unitOfWork.Users.GetById(model.Id);
            user=_mapper.Map(model,user);
            user.Team = null;
            _unitOfWork.Users.Update(user);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = Roles.Administrator)]
        [AjaxOnly]
        public IActionResult AssignRole(string userId)
        {
            var user = _unitOfWork.Users.GetById(userId);
            if (user == null)
                return NotFound();
            var team = _unitOfWork.Teams.GetAll();
            var teamList=_mapper.Map<List<SelectListItem>>(team);
            var roles = GetRoles().Select(e=>new SelectListItem
            {
                Text=e.RoleName,
                Value=e.RoleName

            }).ToList();
            var model = new AssignRoleViewModel()
            {
                RolesList = roles,
                UserId = user.Id,
                TeamList=teamList,
                RoleName=_unitOfWork.Users.GetUserRole(userId).GetAwaiter().GetResult()
                
            };
            return PartialView("_AssignRolePartial", model);
        }
        [HttpPost]
        public async Task<IActionResult> AssignRole(AssignRoleViewModel model)
        {
            if (!ModelState.IsValid)
                BadRequest(ModelState);
            if (model.TeamId !=null)
            {
                var user = _unitOfWork.Users.Find(e => e.Id == model.UserId);
                user.TeamId=model.TeamId;
            }
             var result=await _unitOfWork.Users.AssignRole(model.UserId, model.RoleName);
            if (result)
            {
                _unitOfWork.Complete();
                return RedirectToAction("Index");
            }
            else
                return BadRequest();
        }
        private List<RoleDataViewModel> GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return roles.Select(r => new RoleDataViewModel { RoleName = r.Name, RoleId = r.Id }).ToList();
        }

    }
}
