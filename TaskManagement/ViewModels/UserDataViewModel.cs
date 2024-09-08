using Microsoft.AspNetCore.Mvc.Rendering;

namespace TaskManagement.ViewModels
{
    public class UserDataViewModel
    {
        public string Id {  get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? TeamName { get; set; }
        public int? TeamId { get; set; } 
        public List<SelectListItem>? TeamList { get; set; }
    }
}
