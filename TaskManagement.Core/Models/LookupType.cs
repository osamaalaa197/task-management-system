using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Models
{
    public class LookupType:BaseClass
    {
        public string Type { get; set; } = null!;
        public bool HasChildren { get; set; }
        public int? ChildTypeId { get; set; }
    }
}
