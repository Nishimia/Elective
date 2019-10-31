using ElectiveDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elective.Models.SubjectModels
{
	public class SubjectViewModel
	{
		public string SubjectId { get; set; }

		[Display(Name = "Subject name")]
		public string SubjectName { get; set; }
	}
}