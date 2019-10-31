using ElectiveBLL.Interfaces.ModelInterfaces;

namespace ElectiveBLL.Models
{
	/// <summary>
	/// Entity type for course's subjects
	/// </summary>
	public class CourseSubject : CourseSubject<string>
	{
		
	}

	/// <summary>
	/// Entity type for course's subjects
	/// </summary>
	public class CourseSubject<TKey> : ICourseSubject<TKey>
	{
		/// <summary>
		///		CourseId for course
		/// </summary>
		public virtual TKey CourseId { get; set; }

		/// <summary>
		///		SubjectId for subject that describes cource
		/// </summary>
		public virtual TKey SubjectId { get; set; }
	}
}
