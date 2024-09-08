using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Core.Models;

namespace TaskManagement
{
    public class TaskReportFilters
    {
        public DateTime? EndDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
        public int? TeamId { get; set; }
        public string? TaskPriority { get; set; }
        public IEnumerable<SelectListItem>? TaskStatusList { get; set; }
        public string? TaskPriorityLevel { get; set; }
        public IEnumerable<SelectListItem>? TaskPriorityLevelList { get; set; }
        public IEnumerable<SelectListItem>? UsersList { get; set; }
        public IEnumerable<SelectListItem>? TeamList { get; set; }
        public List<RawData>? datas { get; set; }

    }
}
