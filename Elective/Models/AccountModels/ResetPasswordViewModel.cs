using ElectiveBLL;
using System.ComponentModel.DataAnnotations;

namespace Elective.Models.AccountModels
{
	public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(Constants.PASSWORD_MAX_LENGTH, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = Constants.PASSWORD_MIN_LENGTH)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
