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
	///		Interfase that expose basic functionality for subject manager
	/// </summary>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface ISubjectManager<TSubject, TKey> : IDisposable
		where TSubject : ISubject<TKey>
	{
		/// <summary>
		///		Gets and sets subject validator
		/// </summary>
		IIdentityValidator<TSubject> SubjectValidator { get; set; }
		
		/// <summary>
		///		Gets all Subjects
		/// </summary>
		IQueryable<TSubject> Subjects { get; }

		/// <summary>
		///		Creates subject async
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		Task<IdentityResult> CreateAsync(TSubject subject);

		/// <summary>
		///		Updates subject async
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		Task<IdentityResult> UpdateAsync(TSubject subject);

		/// <summary>
		///		Mark subject for deletion async
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		Task<IdentityResult> DeleteAsync(TSubject subject);

		/// <summary>
		///		Returns subject by it's id
		/// </summary>
		/// <param name="subjectId"></param>
		/// <returns></returns>
		Task<TSubject> FindByIdAsync(TKey subjectId);

		/// <summary>
		///		Returns subject by it's name
		/// </summary>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task<TSubject> FindByNameAsync(string subjectName);
	}
}
