using ElectiveBLL.Interfaces.ModelInterfaces;

namespace ElectiveBLL.Models
{
	/// <summary>
	///		Entity type for course's students
	/// </summary>
	public class UserCourse : UserCourse<string>
	{
	}

	/// <summary>
	///		Entity type for course's students
	/// </summary>
	public class UserCourse<TKey> : IUserCourse<TKey>
	{
		/// <summary>
		///		Student mark for course
		/// </summary>
		public int? StudentMark { get; set; }

		///	<summary>
		///		Mark that user is a tutor
		/// </summary>
		public bool IsTutor { get; set; }

		/// <summary>
		///		StudentId for students enrolled in course
		/// </summary>
		public virtual TKey UserId { get; set; }

		/// <summary>
		///		CourseId for course
		/// </summary>
		public virtual TKey CourseId { get; set; }
	}
}
