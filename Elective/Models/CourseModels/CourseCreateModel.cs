using Elective.Validation;
using Elective.Views.UserModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Elective.Models.CourseModels
{
	public class CourseCreateModel
	{
		[Required]
		[DataType(DataType.Text)]
		public string CourseName { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[DateLessThan("EndDate", ErrorMessage = "Not valid date, start date should go before end date")]
		[Display(Name = "Start date")]
		public DateTime StartDate { get; set; }

		[Required]
		[DataType(DataType.Date)]
		[Display(Name = "End date")]
		public DateTime EndDate { get; set; }

		[Display(Name = "Subjects")]
		public List<string> Subjects { get; set; }

		[Display(Name = "Tutor")]
		public string TutorId { get; set; }

		public IEnumerable<SelectListItem> TutorSelectList { get; set; }

		public IEnumerable<SelectListItem> SubjectSelectList { get; set; }
	}
}