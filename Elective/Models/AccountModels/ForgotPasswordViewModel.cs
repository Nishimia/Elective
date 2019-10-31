using System.ComponentModel.DataAnnotations;

namespace Elective.Models.AccountModels
{

	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }
	}
}
