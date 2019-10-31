using ElectiveBLL;
using System.ComponentModel.DataAnnotations;

namespace Elective.Models.AccountModels
{
	public class RegisterViewModel
	{
		[Required]
		[StringLength(Constants.USER_LOGIN_MAX_LENGTH, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = Constants.USER_LOGIN_MIN_LENGTH)]
		[DataType(DataType.Text)]
		[Display(Name = "Login")]
		public string Login { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(Constants.USER_NAME_MAX_LENGTH, ErrorMessage = "The {0} must be al least {2} characters long", MinimumLength = Constants.USER_NAME_MIN_LENGTH)]
		[DataType(DataType.Text)]
		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[Required]
		[StringLength(Constants.USER_NAME_MAX_LENGTH, ErrorMessage = "The {0} must be al least {2} characters long", MinimumLength = Constants.USER_NAME_MIN_LENGTH)]
		[DataType(DataType.Text)]
		[Display(Name = "Second name")]
		public string SecondName { get; set; }

		[Required]
		[StringLength(Constants.PASSWORD_MAX_LENGTH, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = Constants.PASSWORD_MIN_LENGTH)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
	}
}