using Elective.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elective.Models.JournalModels
{
	public class JournalEditModel
	{
		public string CourseId { get; set; }
		public List<UserJournalModel> UserJournalModels { get; set; }
	}
}