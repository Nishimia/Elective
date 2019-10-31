namespace ElectiveBLL.Interfaces.ModelInterfaces
{
	/// <summary>
	///     Minimal interface for a userCourse
	/// </summary>
	public interface IUserCourse<TKey>
	{
		/// <summary>
		///		Student mark for course
		/// </summary>
		int? StudentMark { get; set; }

		///	<summary>
		///		Mark that user is a tutor
		/// </summary>
		bool IsTutor { get; set; }

		/// <summary>
		///		StudentId for students enrolled in course
		/// </summary>
		TKey UserId { get; set; }

		/// <summary>
		///		CourseId for course
		/// </summary>
		TKey CourseId { get; set; }
	}
}
