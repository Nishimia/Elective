using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elective.Models.UserModels
{
	public class UserJournalModel
	{
		public string UserId { get; set; }

		[Display(Name = "Name")]
		public string UserName { get; set; }

		[Display(Name = "Mark")]
		[Range(0,100)]
		public int? Mark { get; set; }
	}
}