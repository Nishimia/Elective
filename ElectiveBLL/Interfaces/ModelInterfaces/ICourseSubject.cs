namespace ElectiveBLL.Interfaces.ModelInterfaces
{
	/// <summary>
	///     Minimal interface for a courseSubject
	/// </summary>
	public interface ICourseSubject<TKey>
	{

		/// <summary>
		///		CourseId for course
		/// </summary>
		TKey CourseId { get; set; }

		/// <summary>
		///		SubjectId for subject that describes cource
		/// </summary>
		TKey SubjectId { get; set; }
	}
}
