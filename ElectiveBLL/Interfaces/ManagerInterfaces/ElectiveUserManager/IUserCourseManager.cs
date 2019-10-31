using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.ManagerInterfaces
{
	/// <summary>
	///		Interfase that map users to their courses
	/// </summary>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface IUserCourseManager<TUser, TCourse, TKey>
		where TUser : IUser<TKey>
		where TCourse : ICourse<TKey>
	{
		Task<bool> IsInCourseAsync(TKey userId, TKey courseId);
		Task<IdentityResult> AddToCourseAsync(TKey userId, TKey courseId, bool isTutor);
		Task<IdentityResult> RemoveFromCourseAsync(TKey userId, TKey courseId);
		Task<IQueryable<TCourse>> GetUserCoursesAsync(TKey userId);
		Task<IQueryable<TCourse>> GetUserCoursesAsync(TKey userId, bool isTutored);
		Task<int?> GetMarkAsync(TKey userId, TKey courseId);
		Task<IdentityResult> SetMarkAsync(TKey userId, TKey courseId, int? mark);

		/// <summary>
		///		Ban user account
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<IdentityResult> BanUserAsync(TKey userId);

		/// <summary>
		///		Unban user account
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		Task<IdentityResult> UnbanUserAsync(TKey userId);
	}
}
