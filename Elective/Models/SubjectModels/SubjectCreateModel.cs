using ElectiveBLL;
using System.ComponentModel.DataAnnotations;

namespace Elective.Models.SubjectModels
{
	public class SubjectCreateModel
	{
		[Required]
		[DataType(DataType.Text)]
		[StringLength(Constants.TITLE_MAX_LENGTH, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = Constants.TITLE_MIN_LENGTH)]
		[Display(Name = "Subject name")]
		public string SubjectName { get; set; }
	}
}