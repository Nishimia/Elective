using Elective.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elective.Models.CourseModels
{
	public class CourseViewModel
	{
		public string CourseId { get; set; }

		[Display(Name = "Course Name")]
		public string CourseName { get; set; }

		[Display(Name = "Start")]
		public DateTime StartDate { get; set; }

		[Display(Name = "End")]
		public DateTime EndDate { get; set; }

		[Display(Name = "Days")]
		public int Days { get; set; }

		[Display(Name = "Users")]
		public int UserAmount { get; set; }

		[Display(Name = "Subjects")]
		public string Subjects { get; set; }

	}
}