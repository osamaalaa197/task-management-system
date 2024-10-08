﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core;
using TaskManagement.Core.Models;
using TaskManagement.EF;
using TaskManagement.Extensions;
using TaskManagement.Filter;
using TaskManagement.ViewModels;

namespace TaskManagement.Controllers
{
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtector _dataProtector;

        public CommentController(IUnitOfWork unitOfWork, IDataProtectionProvider dataProtector)
        {
            _unitOfWork=unitOfWork;
            _dataProtector = dataProtector.CreateProtector("TaskManagement");

        }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        [AjaxOnly]
        public IActionResult AddComment(string taskId)
        {
            CommentViewModel model = new();
            return PartialView("_AddComment", model);
        }
        [HttpPost]
        public IActionResult AddComment(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var task = _unitOfWork.Assignments.Find(e => e.Id == model.TaskId);
            if (task == null)
                return NotFound();
            var comment = new Comments()
            {
                CreatedById = User.GetUserId(),
                CreatedOn = DateTime.Now,
                comment = model.Comment,
                AssignmentId = model.TaskId
            };
            _unitOfWork.Comments.Add(comment);
            _unitOfWork.Complete();
            return RedirectToAction("GetAssignment", "Assignment", new { id = _dataProtector.Protect(model.TaskId.ToString()) });
        }
    }
}
