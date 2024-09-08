namespace TaskManagement.ViewModels
{
    public class AddMembersFormViewModel
    {
        public int TeamId { get; set; }
        public string MembersPhone { get; set; }
        public List<TeamMembersDataViewModel> teamMembers { get; set; } = new();
    }
}
