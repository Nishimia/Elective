using System.ComponentModel.DataAnnotations;

namespace Elective.Models.AccountModels
{
	public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}