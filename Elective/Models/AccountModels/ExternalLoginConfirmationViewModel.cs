using System.ComponentModel.DataAnnotations;

namespace Elective.Models.AccountModels
{
	public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
