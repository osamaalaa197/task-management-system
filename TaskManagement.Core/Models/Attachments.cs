using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Models;

namespace TaskManagement.Core.Models
{
    public class Attachments:BaseClass
    {
        public int AssignmentId {  get; set; }
        public Assignment Assignment { get; set; }
        public string FilePath { get; set; }
    }
}
