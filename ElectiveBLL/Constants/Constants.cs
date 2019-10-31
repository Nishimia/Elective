using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectiveBLL
{
	/// <summary>
	///		Class that stores all constants I use
	/// </summary>
	public static class Constants
	{
		public const int USER_NAME_MAX_LENGTH = 100;
		public const int USER_NAME_MIN_LENGTH = 3;
		public const int TITLE_MAX_LENGTH = 250;
		public const int TITLE_MIN_LENGTH = 5;
		public const string STUDENT_ROLE_NAME = "student";
		public const string TUTOR_ROLE_NAME = "tutor";
		public const string ADMIN_ROLE_NAME = "administrator";
		public const int PASSWORD_MAX_LENGTH = 16;
		public const int PASSWORD_MIN_LENGTH = 6;
		public const int USER_LOGIN_MAX_LENGTH = 16;
		public const int USER_LOGIN_MIN_LENGTH = 6;
	}
}
