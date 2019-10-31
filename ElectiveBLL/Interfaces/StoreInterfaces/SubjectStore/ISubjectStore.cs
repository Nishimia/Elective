using ElectiveBLL.Interfaces.ModelInterfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.StoreInterfaces
{
	/// <summary>
	///		Interface that exposes basic subject management
	/// </summary>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface ISubjectStore<TSubject, TKey> : IDisposable
		where TSubject : ISubject<TKey>
	{
		/// <summary>
		///		Insert a new element
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		Task CreateAsync(TSubject model);

		/// <summary>
		///		Update an element
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		Task UpdateAsync(TSubject model);

		/// <summary>
		///		Mark element for deletion
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		Task DeleteAsync(TSubject model);

		/// <summary>
		///		Find an element by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<TSubject> FindByIdAsync(TKey id);

		/// <summary>
		///		Find an element by name
		/// </summary>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task<TSubject> FindByNameAsync(string subjectName);

		/// <summary>
		///		IQueryable Subjects
		/// </summary>
		IQueryable<TSubject> Subjects { get; }
	}
}
