using System.ComponentModel.DataAnnotations;
using TaskManagement.Core.Consts;

namespace TaskManagement.ViewModels
{
    public class CommentViewModel
    {
        public int TaskId { get; set; }
        [Display(Name = "Add Your Comment")]
        [MaxLength(2000,ErrorMessage = "lengthError"),RegularExpression(RegexPatterns.CharactersOnly_ArabicAndEnglish, ErrorMessage = Errors.SpecialCharactersNotAllowed)]
        public string Comment {  get; set; }
    }
}
