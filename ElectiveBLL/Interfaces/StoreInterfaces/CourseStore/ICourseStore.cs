using ElectiveBLL.Interfaces.ModelInterfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.StoreInterfaces
{
	/// <summary>
	///		Interface that exposes basic course management
	/// </summary>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface ICourseStore<TCourse, TKey> : IDisposable
		where TCourse : ICourse<TKey>
	{
		/// <summary>
		///		Insert a new element
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		Task CreateAsync(TCourse course);

		/// <summary>
		///		Update an element
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		Task UpdateAsync(TCourse course);

		/// <summary>
		///		Mark element for deletion
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		Task DeleteAsync(TCourse course);

		/// <summary>
		///		Find an element by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<TCourse> FindByIdAsync(TKey courseId);

		/// <summary>
		///		IQueryable Courses
		/// </summary>
		IQueryable<TCourse> Courses { get; }
	}
}
