using ElectiveBLL.Interfaces.ManagerInterfaces;
using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces.CourseStore;
using ElectiveBLL.Models;
using ElectiveDAL.Stores;
using ElectiveDAL.Validators;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Managers
{
	public class CourseManager : CourseManager<Course, ElectiveUser, Subject, string>
	{
		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="store"></param>
		public CourseManager(ICourseStore<Course, string> store)
			: base(store)
		{
		}

		public static CourseManager Create(IdentityFactoryOptions<CourseManager> options, IOwinContext context)
		{
			return new CourseManager(new CourseStore<Course, ElectiveUser, Subject, CourseSubject, UserCourse>(context.Get<DbContext>()));
		}
	}


	public class CourseManager<TCourse, TUser, TSubject, TKey> :
		ICourseManager<TCourse, TKey>,
		ICourseSubjectManager<TSubject, TKey>,
		ICourseUserManager<TCourse, TUser, TKey>
		where TCourse : class, ICourse<TKey>
		where TUser : IElectiveUser<TKey>
		where TSubject : ISubject<TKey>
		where TKey : IEquatable<TKey>
	{
		private IIdentityValidator<TCourse> _courseValidator;
		private bool _disposed;

		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="store"></param>
		public CourseManager(ICourseStore<TCourse, TKey> store)
		{
			Store = store ?? throw new ArgumentNullException(nameof(store));
			CourseValidator = new CourseValidator<TCourse, TKey>();
		}

		/// <summary>
		///		Persistence abstraction that the CourseManager operates against
		/// </summary>
		protected ICourseStore<TCourse, TKey> Store { get; private set; }

		#region ICourseManager

		/// <summary>
		///		Returns an IQueryable of courses if the store is an IQueryableUserStore
		/// </summary>
		public virtual IQueryable<TCourse> Courses
		{
			get
			{
				return Store.Courses;
			}
		}

		/// <summary>
		///		Used to validate courses before changes are saved
		/// </summary>
		public IIdentityValidator<TCourse> CourseValidator
		{
			get
			{
				ThrowIfDisposed();
				return _courseValidator;
			}

			set
			{
				ThrowIfDisposed();
				if (value is null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				_courseValidator = value;
			}
		}

		/// <summary>
		///		Create a course
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public async Task<IdentityResult> CreateAsync(TCourse course)
		{
			ThrowIfDisposed();
			var result = await CourseValidator.ValidateAsync(course).WithCurrentCulture();
			if (!result.Succeeded)
			{
				return result;
			}
			await Store.CreateAsync(course).WithCurrentCulture();
			return IdentityResult.Success;
		}

		/// <summary>
		///		Update a course
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public async Task<IdentityResult> UpdateAsync(TCourse course)
		{
			ThrowIfDisposed();
			if (course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			var result = await CourseValidator.ValidateAsync(course).WithCurrentCulture();
			if (!result.Succeeded)
			{
				return result;
			}
			await Store.UpdateAsync(course).WithCurrentCulture();
			return IdentityResult.Success;
		}

		/// <summary>
		///		Delete a course
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public async Task<IdentityResult> DeleteAsync(TCourse course)
		{
			ThrowIfDisposed();
			await Store.DeleteAsync(course).WithCurrentCulture();
			return IdentityResult.Success;
		}

		/// <summary>
		///		Find course by id
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public async Task<TCourse> FindByIdAsync(TKey courseId)
		{
			ThrowIfDisposed();
			return await Store.FindByIdAsync(courseId).WithCurrentCulture();
		}

		/// <summary>
		///		Dispose this object
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region ICourseSubjectManager

		/// <summary>
		///		Returns true if the course is in the specified subject
		/// </summary>
		/// <param name="courseId">course id</param>
		/// <param name="subject">subject name</param>
		/// <returns></returns>
		public async Task<bool> IsInSubject(TKey courseId, string subject)
		{
			ThrowIfDisposed();
			var courseSubjectStore = GetCourseSubjectStore();
			var course = await FindByIdAsync(courseId);
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			return await courseSubjectStore.IsInSubjectAsync(course, subject).WithCurrentCulture();
		}

		/// <summary>
		///		Add a course to a subject
		/// </summary>
		/// <param name="courseId">course id</param>
		/// <param name="subject">subject name</param>
		/// <returns></returns>
		public async Task<IdentityResult> AddToSubjectAsync(TKey courseId, string subjectName)
		{
			ThrowIfDisposed();
			var courseSubjectStore = GetCourseSubjectStore();
			var course = await FindByIdAsync(courseId).WithCurrentCulture();
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			var courseSubjects = courseSubjectStore.GetCourseSubjects(course).ToList();
			if (courseSubjects.Contains(subjectName))
			{
				return new IdentityResult(Resources.Resources.CourseAlreadyInSubject);
			}
			await courseSubjectStore.AddToSubjectAsync(course, subjectName).WithCurrentCulture();
			return await UpdateAsync(course).WithCurrentCulture();
		}

		/// <summary>
		///		Add a course to subjects
		/// </summary>
		/// <param name="courseId">course id</param>
		/// <param name="subjects">list of subject names</param>
		/// <returns></returns>
		public async Task<IdentityResult> AddToSubjectsAsync(TKey courseId, params string[] subjectNames)
		{
			ThrowIfDisposed();
			var courseSubjectStore = GetCourseSubjectStore();
			if (subjectNames is null)
			{
				throw new ArgumentNullException(nameof(subjectNames));
			}
			var course = await Store.FindByIdAsync(courseId).WithCurrentCulture();
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			var courseSubjects = courseSubjectStore.GetCourseSubjects(course);
			foreach (var subjectName in subjectNames)
			{
				if (courseSubjects.Contains(subjectName))
				{
					return new IdentityResult(Resources.Resources.CourseAlreadyInSubject);
				}
				await courseSubjectStore.AddToSubjectAsync(course, subjectName).WithCurrentCulture();
			}
			return await UpdateAsync(course).WithCurrentCulture();
		}

		/// <summary>
		///		Remove course from subject
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subject"></param>
		/// <returns></returns>
		public async Task<IdentityResult> RemoveFromSubjectAsync(TKey courseId, string subject)
		{
			ThrowIfDisposed();
			var courseSubjectStore = GetCourseSubjectStore();
			var course = await FindByIdAsync(courseId).WithCurrentCulture();
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			if (!await courseSubjectStore.IsInSubjectAsync(course, subject).WithCurrentCulture())
			{
				return new IdentityResult(Resources.Resources.CourseNotInSubject);
			}
			await courseSubjectStore.RemoveFromSubjectAsync(course, subject).WithCurrentCulture();
			return await UpdateAsync(course).WithCurrentCulture();
		}

		/// <summary>
		///		Remove course from multiple subjects
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="subjectNames"></param>
		/// <returns></returns>
		public async Task<IdentityResult> RemoveFromSubjectsAsync(TKey courseId, params string[] subjectNames)
		{
			ThrowIfDisposed();
			var courseSubjectStore = GetCourseSubjectStore();
			if(subjectNames is null)
			{
				throw new ArgumentNullException(nameof(subjectNames));
			}
			var course = await FindByIdAsync(courseId).WithCurrentCulture();
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			var courseSubjects = courseSubjectStore.GetCourseSubjects(course);
			foreach (var subjectName in subjectNames)
			{
				if (!courseSubjects.Contains(subjectName))
				{
					return new IdentityResult(Resources.Resources.CourseNotInSubject);
				}
				await courseSubjectStore.RemoveFromSubjectAsync(course, subjectName).WithCurrentCulture();
			}
			return await UpdateAsync(course).WithCurrentCulture();
		}

		/// <summary>
		///		Return subjects of course
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public async Task<IQueryable<string>> GetAllSubjectsAsync(TKey courseId)
		{
			ThrowIfDisposed();
			var courseSubjectStore = GetCourseSubjectStore();
			var course = await FindByIdAsync(courseId).WithCurrentCulture();
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			return courseSubjectStore.GetCourseSubjects(course);
		}

		#endregion

		#region ICourseUserStore
		public async Task<TUser> GetTutorAsync(TKey courseId)
		{
			ThrowIfDisposed();
			var courseUserStore = GetCourseUserStore();
			var course = await FindByIdAsync(courseId);
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			return courseUserStore.GetTutor(course);
		}

		public async Task<IQueryable<TUser>> GetStudentsByCourseAsync(TKey courseId)
		{
			ThrowIfDisposed();
			var courseUserStore = GetCourseUserStore();
			var course = await FindByIdAsync(courseId);
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.Resources.CourseIdNotFound,
					courseId));
			}
			return courseUserStore.GetStudentsByCourse(course);
		}

		#endregion


		/// <summary>
		///		Return ICourseSubjectStore from existing Store
		/// </summary>
		/// <returns></returns>
		private ICourseSubjectStore<TCourse, TSubject, TKey> GetCourseSubjectStore()
		{
			var cast = Store as ICourseSubjectStore<TCourse, TSubject, TKey>;
			if (cast is null)
			{
				throw new NotSupportedException(string.Format(Resources.Resources.StoreDoesNotImplementInterface,
					nameof(ICourseSubjectStore<TCourse, TSubject, TKey>)));
			}
			return cast;
		}

		/// <summary>
		///		Return ICourseSubjectStore from existing Store
		/// </summary>
		/// <returns></returns>
		private ICourseUserStore<TCourse, TUser, TKey> GetCourseUserStore()
		{
			var cast = Store as ICourseUserStore<TCourse, TUser, TKey>;
			if (cast is null)
			{
				throw new NotSupportedException(string.Format(Resources.Resources.StoreDoesNotImplementInterface,
					nameof(ICourseUserStore<TCourse, TUser, TKey>)));
			}
			return cast;
		}

		/// <summary>
		///		Throw exception if object is disposed
		/// </summary>
		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		///		When disposing, actually dipose the store
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				Store.Dispose();
				_disposed = true;
			}
		}


	}
}
