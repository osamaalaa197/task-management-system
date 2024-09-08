using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Core.Models
{
    public class AuditTrail
    {
        public int Id { get; set; }
        public string ActionType { get; set; }
        public string ActionDescription { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int? TaskId { get; set; }
        public Assignment Task { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
