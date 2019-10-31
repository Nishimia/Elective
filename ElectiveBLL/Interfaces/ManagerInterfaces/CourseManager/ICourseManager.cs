using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.ManagerInterfaces
{
	/// <summary>
	///		Interface for course manager
	/// </summary>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface ICourseManager<TCourse, TKey> : IDisposable
		where TCourse : ICourse<TKey>
	{
		/// <summary>
		///		Gets and sets validator
		/// </summary>
		IIdentityValidator<TCourse> CourseValidator { get; set; }

		/// <summary>
		///		Returns all courses
		/// </summary>
		IQueryable<TCourse> Courses { get; }

		/// <summary>
		///		Creates course async
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		Task<IdentityResult> CreateAsync(TCourse course);

		/// <summary>
		///		Updates course async
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		Task<IdentityResult> UpdateAsync(TCourse course);

		/// <summary>
		///		 Marks course for deletion
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		Task<IdentityResult> DeleteAsync(TCourse course);

		/// <summary>
		///		Returns course by it's id
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Task<TCourse> FindByIdAsync(TKey courseId);
	}
}
