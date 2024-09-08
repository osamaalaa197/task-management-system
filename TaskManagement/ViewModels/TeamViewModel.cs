using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Consts;

namespace TaskManagement.ViewModels
{
    public class TeamViewModel
    {
        public int TeamId { get; set; }
        [Display(Name = "Add Your Team Name")]
        [MaxLength(100, ErrorMessage = "lengthError"), RegularExpression(RegexPatterns.CharactersOnly_ArabicAndEnglish, ErrorMessage = Errors.SpecialCharactersNotAllowed)]
        public string TeamName { get; set; }
        [Display(Name = "Add Your Team Description")]
        [MaxLength(2000, ErrorMessage = "lengthError"), RegularExpression(RegexPatterns.CharactersOnly_ArabicAndEnglish, ErrorMessage = Errors.SpecialCharactersNotAllowed)]
        public string Description { get; set; }
    }
}
