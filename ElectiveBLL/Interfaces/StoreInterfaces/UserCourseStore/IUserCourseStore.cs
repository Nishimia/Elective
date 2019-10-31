using ElectiveBLL.Interfaces.ModelInterfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.StoreInterfaces
{
	/// <summary>
	///		Interface that maps users to their courses
	/// </summary>
	/// <typeparam name="TUser"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface IUserCourseStore<TUser, TCourse, TKey> : IDisposable
		where TUser : IElectiveUser<TKey>
		where TCourse : ICourse<TKey>
	{
		/// <summary>
		///		Add a user to a course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <param name="isTutor"></param>
		/// <returns></returns>
		Task AddToCourseAsync(TUser user, TKey courseId, bool isTutor);

		/// <summary>
		///		Remove user from course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Task RemoveFromCourseAsync(TUser user, TKey courseId);

		/// <summary>
		///		Return users cources
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		IQueryable<TCourse> GetUserCourses(TUser user);

		/// <summary>
		///		If isTutored get tutored courses, else get studied courses
		/// </summary>
		/// <param name="user"></param>
		/// <param name="isTutored"></param>
		/// <returns></returns>
		IQueryable<TCourse> GetUserCourses(TUser user, bool isTutored);


		/// <summary>
		///		Returns true if user is in the course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Task<bool> IsInCourseAsync(TUser user, TKey courseId);

		/// <summary>
		///		Return mark for this course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Task<int?> GetMarkAsync(TUser user, TKey courseId);

		/// <summary>
		///		Sets user mark for course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		Task SetMarkAsync(TUser user, TKey courseId, int? mark);
	}
}
