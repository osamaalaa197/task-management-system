using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.ViewModels
{
    public class AssignmentViewModel
    {
        public string? Key { get; set; }
        [Display(Name = "Add Title")]
        public string Title { get; set; }
        [Display(Name = "Add Description")]
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        [Display(Name = "Select Priority Level")]
        public string PriorityLevel { get; set; }
        public IEnumerable<SelectListItem>? PriorityLevels { get; set; }
        [Display(Name = "Select Assignment Status")]
        public string Status { get; set; }
        public IEnumerable<SelectListItem>? AssignmentStatusList { get; set; }
        public IFormFile? AttachmentFile {  get; set; }
        public List<AttachmentViewModel>? Attachment{ get; set; }
        public string? AssignedUser { get; set; }
        public string? AssignedTeam { get; set; }
        public List<CommentViewModel>? Comment { get; set; }
    }
}
