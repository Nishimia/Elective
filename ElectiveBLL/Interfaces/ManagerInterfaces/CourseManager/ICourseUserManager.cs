using ElectiveBLL.Interfaces.ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectiveBLL.Interfaces.ManagerInterfaces
{
	public interface ICourseUserManager<TCourse, TUser, TKey>
		where TCourse : ICourse<TKey>
		where TUser : IElectiveUser<TKey>
	{
		/// <summary>
		///		Returns tutor of the course
		/// </summary>
		/// <param name="corseId"></param>
		/// <returns></returns>
		Task<TUser> GetTutorAsync(TKey corseId);

		/// <summary>
		///		Returns all students on the course
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		Task<IQueryable<TUser>> GetStudentsByCourseAsync(TKey courseId);
	}
}
