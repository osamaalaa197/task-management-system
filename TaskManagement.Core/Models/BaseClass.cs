using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Core.Models
{
    public class BaseClass
    {
        public int Id { get; set; }
        public string CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
