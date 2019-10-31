using ElectiveBLL.Interfaces.ModelInterfaces;
using System.Linq;

namespace ElectiveBLL.Interfaces.StoreInterfaces.CourseStore
{
	public interface ICourseUserStore<TCourse, TUser, TKey>
		where TUser : IElectiveUser<TKey>
		where TCourse : ICourse<TKey>
	{
		/// <summary>
		///		Returns all course students
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		IQueryable<TUser> GetStudentsByCourse(TCourse course);

		/// <summary>
		///		Returns tutor of the course
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		TUser GetTutor(TCourse course);
	}
}
