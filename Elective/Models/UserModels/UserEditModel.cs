using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Elective.Models.UserModels
{
	public class UserEditModel
	{
		public string UserId { get; set; }

		[Display(Name = "Login")]
		public string UserName { get; set; }

		[Display(Name = "First name")]
		public string FirstName { get; set; }

		[Display(Name = "Second name")]
		public string SecondName { get; set; }

		[Display(Name = "Tutor")]
		public bool IsTutor { get; set; }

		[Display(Name = "Ban")]
		public bool IsBanned { get; set; }
	}
}