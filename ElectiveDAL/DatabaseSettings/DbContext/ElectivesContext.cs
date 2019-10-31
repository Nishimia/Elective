using ElectiveBLL;
using ElectiveBLL.Models;
using ElectiveDAL.DatabaseSettings.DbContextInitializer;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace ElectiveDAL.DatabaseSettings.DbContext
{
	/// <summary>
	/// Database context class implementation with identity
	/// </summary>
	public class ElectivesContext : IdentityDbContext<ElectiveUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
	{
		/// <summary>
		/// Sets max length for names in database
		/// </summary>


		/// <summary>
		/// Static constructor
		/// </summary>
		static ElectivesContext()
		{
			//Setting initializer for database
			Database.SetInitializer(new ElectivesContextInitializer());
		}

		/// <summary>
		/// Creates new context
		/// </summary>
		/// <param name="dbName">Connection string or database name</param>
		public ElectivesContext(string dbName) :
			base(dbName)
		{

		}

		public static ElectivesContext Create()
		{
			return new ElectivesContext("DefaultConnection");
		}

		#region Database tables and configuring

		public virtual IDbSet<UserCourse> CourseEnrollments { get; set; }
		public virtual IDbSet<Course> Courses { get; set; }
		public virtual IDbSet<CourseSubject> CourseSubjects { get; set; }
		public virtual IDbSet<Subject> Subjects { get; set; }

		/// <summary>
		///		Configure model using Fluent API
		/// </summary>
		/// <param name="modelBuilder"></param>
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			var userCourses = modelBuilder.Entity<UserCourse>().ToTable("userCourses");
			userCourses.HasKey(ce => new { ce.CourseId, ce.UserId });
			userCourses.Property(ce => ce.StudentMark).IsOptional();

			var courses = modelBuilder.Entity<Course>().ToTable("Courses");
			courses.HasKey(c => c.Id);
			courses.HasMany(c => c.CourseSubjects).WithRequired().HasForeignKey(cs => cs.CourseId);
			courses.HasMany(c => c.EnrolledInCourseStudents).WithRequired().HasForeignKey(ce => ce.CourseId);
			courses.Property(c => c.CourseName).IsRequired();
			courses.Property(c => c.CourseName).HasMaxLength(Constants.TITLE_MAX_LENGTH);

			var courceSubjects = modelBuilder.Entity<CourseSubject>().ToTable("CourseSubjects");
			courceSubjects.HasKey(cs => new { cs.CourseId, cs.SubjectId });

			var users = modelBuilder.Entity<ElectiveUser>().ToTable("Users");
			users.HasKey(u => u.Id);
			users.HasMany(u => u.EnrolledInCourses).WithRequired().HasForeignKey(ec => ec.UserId);
			users.Property(u => u.FirstName).HasMaxLength(Constants.USER_NAME_MAX_LENGTH);
			users.Property(u => u.SecondName).HasMaxLength(Constants.USER_NAME_MAX_LENGTH);

			var subjects = modelBuilder.Entity<Subject>().ToTable("Subjects");
			subjects.HasKey(s => s.Id);
			subjects.HasMany(s => s.SubjectCourses).WithRequired().HasForeignKey(cs => cs.SubjectId);
			subjects.HasIndex(s => s.SubjectName).IsUnique();
			subjects.Property(s => s.SubjectName).HasMaxLength(Constants.TITLE_MAX_LENGTH);
		}

		#endregion
	}
}
