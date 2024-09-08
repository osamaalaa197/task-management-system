using Microsoft.AspNetCore.Identity;

namespace TaskManagement.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
