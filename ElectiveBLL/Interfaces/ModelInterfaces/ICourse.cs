using System;

namespace ElectiveBLL.Interfaces.ModelInterfaces
{
	/// <summary>
	///     Minimal interface for a course with id and name
	/// </summary>
	public interface ICourse<TKey>
	{
		/// <summary>
		///		Course ID
		/// </summary>
		TKey Id { get; set; }

		/// <summary>
		///		Name of the course
		/// </summary>
		string CourseName { get; set; }

		/// <summary>
		///		StartDate of the course
		/// </summary>
		DateTime StartDate { get; set; }

		/// <summary>
		///		EndDate of the course
		/// </summary>
		DateTime EndDate { get; set; }
	}
}
