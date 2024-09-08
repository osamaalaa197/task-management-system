using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Core.Models
{
    public class RawData
    {
        public string Id {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime createdOn { get; set; }
        public string PriorityLevel { get; set; }
        public string Status { get; set; }
        public string? AssignedToUser { get; set; }

    }
}
