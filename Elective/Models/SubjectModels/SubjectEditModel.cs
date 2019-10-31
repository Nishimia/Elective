using ElectiveBLL;
using ElectiveDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elective.Models.SubjectModels
{
	public class SubjectEditModel
	{
		public string SubjectId { get; set; }

		[Required]
		[DataType(DataType.Text)]
		[StringLength(Constants.TITLE_MAX_LENGTH, ErrorMessage = "The {0} must be at least {2} characters long", MinimumLength = Constants.TITLE_MIN_LENGTH)]
		[Display(Name = "Subject name")]
		public string SubjectName { get; set; }
	}
}