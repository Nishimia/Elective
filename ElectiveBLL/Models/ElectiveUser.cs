using ElectiveBLL.Interfaces.ModelInterfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectiveBLL.Models
{
	/// <summary>
	///		Represent a user entity
	/// </summary>
	public class ElectiveUser : ElectiveUser<string, UserCourse, IdentityUserLogin, IdentityUserRole, IdentityUserClaim> 
	{
		/// <summary>
		///		Constructor
		/// </summary>
		public ElectiveUser()
			: base()
		{
			Id = Guid.NewGuid().ToString();
		}

		/// <summary>
		///		Creates cookie for identification
		/// </summary>
		public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ElectiveUser, string> manager)
		{
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			return userIdentity;
		}
	}

	/// <summary>
	///		Represent a user entity
	/// </summary>
	public class ElectiveUser<TKey, TUserCourse, TLogin, TRole, TClaim> : IdentityUser<TKey, TLogin, TRole, TClaim>, IElectiveUser<TKey>
		where TUserCourse : IUserCourse<TKey>
		where TLogin : IdentityUserLogin<TKey>
		where TRole : IdentityUserRole<TKey>
		where TClaim : IdentityUserClaim<TKey>
	{
		/// <summary>
		///		Constructor
		/// </summary>
		public ElectiveUser()
		{
			EnrolledInCourses = new List<TUserCourse>();
		}
		
		/// <summary>
		///		Return true if user is banned
		/// </summary>
		public bool Banned { get; set; }

		/// <summary>
		///		User's first name
		/// </summary>
		public string FirstName { get; set; }

		/// <summary>
		///		User's second name
		/// </summary>
		public string SecondName { get; set; }

		/// <summary>
		///		Courses user enroled in
		/// </summary>
		public virtual ICollection<TUserCourse> EnrolledInCourses { get; set; }
	}
}
