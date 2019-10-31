using ElectiveBLL.Interfaces.ModelInterfaces;
using System;
using System.Collections.Generic;

namespace ElectiveBLL.Models
{
	/// <summary>
	/// Represent a subject entity
	/// </summary>
	public class Subject : Subject<string, CourseSubject>
	{
		/// <summary>
		///		Constructor
		/// </summary>
		public Subject()
			: base()
		{
			Id = Guid.NewGuid().ToString();
		}
	}

	/// <summary>
	/// Represent a subject entity
	/// </summary>
	public class Subject<TKey, TCourseSubject> : ISubject<TKey>
		where TCourseSubject : CourseSubject<TKey>
	{
		/// <summary>
		///		Constructor
		/// </summary>
		public Subject()
		{
			SubjectCourses = new List<TCourseSubject>();
		}

		/// <summary>
		///		Subject ID
		/// </summary>
		public TKey Id { get; set; }

		/// <summary>
		///		Name of the subject
		/// </summary>
		public string SubjectName { get; set; }

		/// <summary>
		///		Courses that matches the subject
		/// </summary>
		public virtual ICollection<TCourseSubject> SubjectCourses { get; set; }
	}
}
