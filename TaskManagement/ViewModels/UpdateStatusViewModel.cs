using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.ViewModels
{
    public class UpdateStatusViewModel
    {
        public string AssignmentId { get; set; }
        [Display(Name = "Select New Status")]
        public string NewStatus { get; set; }
        public List<SelectListItem>? StatusList { get; set; }
    }
}
