using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace TaskManagement.ViewModels
{
    public class AssignFormViewModel
    {
        [RequiredIf("IsAssignedToTeam", false, ErrorMessage = "select User ")]
        public string? UserId { get; set; }
        public string AssignmentId { get; set; }
        public List<SelectListItem>? ListUser { get; set; } = new();
        [RequiredIf("IsAssignedToTeam",true,ErrorMessage ="select Team")]
        public int? TeamId { get; set; }
        public bool IsAssignedToTeam { get; set; }
        public List<SelectListItem>? ListTeam { get; set; } = new();
        public List<SelectListItem>? ListTeamMembers { get; set; } = new();
        public string? MemberId { get; set; }

    }
}
