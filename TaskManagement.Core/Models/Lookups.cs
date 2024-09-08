using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Core.Models
{
    public class Lookups:BaseClass
    {
        public string Value { get; set; } = null!;
        public int LookupTypeId { get; set; }
        public LookupType? LookupType { get; set; }
    }
}
