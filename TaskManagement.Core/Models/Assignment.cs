using System.Net.Mail;
using TaskManagement.Core.Models;

namespace TaskManagement.Models
{
    public class Assignment:BaseClass
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string PriorityLevel { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
        public string? AssignedToUserId { get; set; }
        public ApplicationUser? AssignedToUser { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<Attachments> Attachments { get; set; }
        public ICollection<Comments> Comments { get; set; }


    }
}
