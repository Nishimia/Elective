using ElectiveBLL.Interfaces.ModelInterfaces;
using System;
using System.Collections.Generic;

namespace ElectiveBLL.Models
{
	public class Course : Course<string, CourseSubject, UserCourse>
	{
		/// <summary>
		///		Constructor
		/// </summary>
		public Course()
			: base()
		{
			Id = Guid.NewGuid().ToString();
		}
	}

	/// <summary>
	///		Represent a course entity
	/// </summary>
	public class Course<TKey, TCourseSubject, TUserCourse> : ICourse<TKey>
		where TCourseSubject : CourseSubject<TKey>
		where TUserCourse : UserCourse<TKey>
	{
		/// <summary>
		///		Constructor
		/// </summary>
		public Course()
		{
			EnrolledInCourseStudents = new List<TUserCourse>();
			CourseSubjects = new List<TCourseSubject>();
		}

		/// <summary>
		///		Course ID
		/// </summary>
		public TKey Id { get; set; }

		/// <summary>
		///		Name of the course
		/// </summary>
		public string CourseName { get; set; }

		/// <summary>
		///		StartDate of the course
		/// </summary>
		public DateTime StartDate { get; set; }

		/// <summary>
		///		EndDate of the course
		/// </summary>
		public DateTime EndDate { get; set; }

		/// <summary>
		///		Navigation property for students enrolled in course 
		/// </summary>
		public virtual ICollection<TUserCourse> EnrolledInCourseStudents { get; set; }

		/// <summary>
		///		Navigation property for subjects describing course
		/// </summary>
		public virtual ICollection<TCourseSubject> CourseSubjects { get; set; }
	}
}
