using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elective.Models.CourseModels
{
	public class UserCourseViewModel
	{
		public string CourseId { get; set; }
		public string CourseName { get; set; }
		public int? Mark { get; set; }
	}
}