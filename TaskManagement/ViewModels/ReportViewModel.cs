using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Models;

namespace TaskManagement.ViewModels
{
    public class ReportViewModel
    {
        [Display(Name = "from")] 
        public string From { get; set; } = null!;

        [Display(Name = "to")] 
        public string To { get; set; } = null!;
        public string TaskStatus { get; set; }
        public IEnumerable<SelectListItem>? TaskStatusList { get; set; }
        [Display(Name = "CaseStatus")]
        public string TaskPriorityLevelId { get; set; }
        public IEnumerable<SelectListItem>? TaskPriorityLevelList { get; set; }
        public string? UserId { get; set; }
        public int? TeamId { get; set; }
        public List <RawData> datas { get; set; }   
    }
}
