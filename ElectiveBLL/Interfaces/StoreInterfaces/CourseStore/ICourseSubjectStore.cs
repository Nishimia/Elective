using ElectiveBLL.Interfaces.ModelInterfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.StoreInterfaces
{
	/// <summary>
	///		Interface that maps courses to their subjects
	/// </summary>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public interface ICourseSubjectStore<TCourse, TSubject, TKey>
		where TCourse : ICourse<TKey>
		where TSubject : ISubject<TKey>
	{

		/// <summary>
		///		Adds a subject to a course
		/// </summary>
		/// <param name="course"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task AddToSubjectAsync(TCourse course, string subjectName);

		/// <summary>
		///		 Removes the subject from the course
		/// </summary>
		/// <param name="course"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task RemoveFromSubjectAsync(TCourse course, string subjectName);

		/// <summary>
		///		Returns the subjects for this course
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		IQueryable<string> GetCourseSubjects(TCourse course);

		/// <summary>
		///		Returns true if a course has the subject
		/// </summary>
		/// <param name="course"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		Task<bool> IsInSubjectAsync(TCourse course, string subjectName);
	}
}
