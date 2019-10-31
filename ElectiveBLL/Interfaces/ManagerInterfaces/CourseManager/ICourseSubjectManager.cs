using ElectiveBLL.Interfaces.ModelInterfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.ManagerInterfaces
{
	/// <summary>
	///		Interface that map courses to their subjects
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	public interface ICourseSubjectManager<TSubject, TKey> : IDisposable
		where TSubject : ISubject<TKey>
	{
		/// <summary>
		///		Returns true if course is in subject
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task<bool> IsInSubject(TKey courseId, string subjectName);

		/// <summary>
		///		Adds course to the subject
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task<IdentityResult> AddToSubjectAsync(TKey courseId, string subjectName);

		/// <summary>
		///		Adds course to a bunch of subjects
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subjectNames"></param>
		/// <returns></returns>
		Task<IdentityResult> AddToSubjectsAsync(TKey courseId, params string[] subjectNames);

		/// <summary>
		///		 Removes course from subject
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task<IdentityResult> RemoveFromSubjectAsync(TKey courseId, string subjectName);

		/// <summary>
		///		Removes course from several subjects
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subjectNames"></param>
		/// <returns></returns>
		Task<IdentityResult> RemoveFromSubjectsAsync(TKey courseId, params string[] subjectNames);

		/// <summary>
		///		Gets all subjects course is in
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Task<IQueryable<string>> GetAllSubjectsAsync(TKey courseId);
	}
}
