using Elective.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elective.Models.CourseModels
{
	public class CourseEditModel
	{
		public string CourseId { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string CourseName { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[DateLessThan("EndDate", ErrorMessage = "Not valid date, start date should go before end date")]
		[Display(Name = "Start date")]
		public DateTime StartDate { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Display(Name = "End date")]
		public DateTime EndDate { get; set; }

		[Display(Name = "Subjects")]
		public List<string> Subjects { get; set; }

		[DataType(DataType.Text)]
		[Display(Name = "Tutor")]
		public string TutorId { get; set; }

		public List<SelectListItem> SubjectSelectList { get; set; }

		public List<SelectListItem> TutorSelectList { get; set; }

	}
}