namespace TaskManagement.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string Description { get; set; }
        public List<ApplicationUser> Members { get; set; }=new();
        public List<Assignment> Assignments { get; set; } = new();
    }
}
