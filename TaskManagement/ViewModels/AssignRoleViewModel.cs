using Microsoft.AspNetCore.Mvc.Rendering;
using Org.BouncyCastle.Asn1.X509;
using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Consts;

namespace TaskManagement.ViewModels
{
    public class AssignRoleViewModel
    {
        public string UserId {  get; set; }
        [Required]
        public string RoleName { get; set; }
        public List<SelectListItem>? RolesList { get; set; }
        [RequiredIf("RoleName", Roles.TeamLeader, ErrorMessage = Errors.RequiredTeam)]
        public int? TeamId { get; set; }
        public List<SelectListItem>? TeamList { get; set; }

    }
}
