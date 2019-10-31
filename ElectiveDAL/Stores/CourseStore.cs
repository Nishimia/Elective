using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces.CourseStore;
using ElectiveDAL.Repositories.AbstractRepositories;
using System;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Stores
{
	/// <summary>
	///		Represent a course store
	/// </summary>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TCourseSubject"></typeparam>
	public class CourseStore<TCourse, TUser, TSubject, TCourseSubject, TUserCourse> : CourseStore<TCourse, TUser, TSubject, TCourseSubject, TUserCourse, string>
		where TCourse : class, ICourse<string>
		where TUser : class, IElectiveUser<string>
		where TSubject : class, ISubject<string>
		where TCourseSubject : class, ICourseSubject<string>, new()
		where TUserCourse : class, IUserCourse<string>, new()
	{
		/// <summary>
		///		Constructor that takes DbContext
		/// </summary>
		/// <param name="context"></param>
		public CourseStore(DbContext context)
			: base(context)
		{

		}
	}

	/// <summary>
	///		Represent a course store
	/// </summary>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TCourseSubject"></typeparam>
	/// <typeparam name="TUserCourse"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public class CourseStore<TCourse, TUser, TSubject, TCourseSubject, TUserCourse, TKey> :
		ICourseUserStore<TCourse, TUser, TKey>,
		ICourseStore<TCourse, TKey>,
		ICourseSubjectStore<TCourse, TSubject, TKey>
		where TCourse : class, ICourse<TKey>
		where TUser : class, IElectiveUser<TKey>
		where TSubject : class, ISubject<TKey>
		where TCourseSubject : class, ICourseSubject<TKey>, new()
		where TUserCourse : class, IUserCourse<TKey>, new()
		where TKey : IEquatable<TKey>
	{
		private readonly IDbSet<TCourseSubject> _courseSubjects;
		private readonly IDbSet<TUserCourse> _userCourses;
		private readonly EntityStore<TSubject> _subjectStore;
		private readonly EntityStore<TUser> _userStore;

		private bool _disposed;
		private EntityStore<TCourse> _courseStore;

		/// <summary>
		///		Constructor that takes DbContext
		/// </summary>
		/// <param name="context"></param>
		public CourseStore(DbContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			DisposeContext = true;
			AutoSaveChanges = true;
			_courseStore = new EntityStore<TCourse>(context);
			_subjectStore = new EntityStore<TSubject>(context);
			_userStore = new EntityStore<TUser>(context);
			_userCourses = Context.Set<TUserCourse>();
			_courseSubjects = Context.Set<TCourseSubject>();
		}

		/// <summary>
		///		Context for the store
		/// </summary>
		public DbContext Context { get; private set; }

		/// <summary>
		///		If true will call dispose on the DbContext during Dispose
		/// </summary>
		public bool DisposeContext { get; set; }

		/// <summary>
		///		If true will call SaveChanges after Create/Update/Delete
		/// </summary>
		public bool AutoSaveChanges { get; set; }

		/// <summary>
		///		Returns an IQueryable of courses
		/// </summary>
		public IQueryable<TCourse> Courses
		{
			get
			{
				return _courseStore.EntitySet;
			}
		}

		#region ICourseStore

		/// <summary>
		///		Insert an entity 
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public virtual async Task CreateAsync(TCourse course)
		{
			ThrowIfDisposed();
			if (course == null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			_courseStore.Create(course);
			await SaveChanges().WithCurrentCulture();
		}

		/// <summary>
		///		Update an entity
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public virtual async Task UpdateAsync(TCourse course)
		{
			ThrowIfDisposed();
			if (course == null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			_courseStore.Update(course);
			await SaveChanges().WithCurrentCulture();
		}

		/// <summary>
		///		Mark an entity for deletion
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public virtual async Task DeleteAsync(TCourse course)
		{
			ThrowIfDisposed();
			if (course == null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			_courseStore.Delete(course);
			await SaveChanges().WithCurrentCulture();
		}

		/// <summary>
		///		Find course by ID
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task<TCourse> FindByIdAsync(TKey courseId)
		{
			ThrowIfDisposed();
			return await _courseStore.GetByIdAsync(courseId);
		}

		/// <summary>
		///		Find courses by name
		/// </summary>
		/// <param name="courseName"></param>
		/// <returns></returns>
		public virtual IQueryable<TCourse> FindCoursesByName(string courseName)
		{
			ThrowIfDisposed();
			if (string.IsNullOrWhiteSpace(courseName))
			{
				throw new ArgumentException(Resources.Resources.ValueCannotBeNullOrEmpty, nameof(courseName));
			}
			return _courseStore.DbEntitySet.Where(c => c.CourseName.ToUpper() == courseName.ToUpper());
		}

		#endregion

		#region ICourseSubjectStore

		/// <summary>
		///		Add subject to course
		/// </summary>
		/// <param name="course"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		public virtual async Task AddToSubjectAsync(TCourse course, string subjectName)
		{
			ThrowIfDisposed();
			if (course == null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			if (string.IsNullOrWhiteSpace(subjectName))
			{
				throw new ArgumentException(Resources.Resources.ValueCannotBeNullOrEmpty, nameof(subjectName));
			}
			var subject = await _subjectStore.DbEntitySet.SingleOrDefaultAsync(s => s.SubjectName.ToUpper() == subjectName.ToUpper()).WithCurrentCulture();
			if (subject is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.SubjectNameNotFound, subjectName));
			}
			var courseSubject = new TCourseSubject() { CourseId = course.Id, SubjectId = subject.Id };
			_courseSubjects.Add(courseSubject);
		}

		/// <summary>
		///		Remove subject from course
		/// </summary>
		/// <param name="course"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		public async Task RemoveFromSubjectAsync(TCourse course, string subjectName)
		{
			ThrowIfDisposed();
			if (course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			if (string.IsNullOrWhiteSpace(subjectName))
			{
				throw new ArgumentException(Resources.Resources.ValueCannotBeNullOrEmpty, nameof(subjectName));
			}
			var subject = await _subjectStore.DbEntitySet.FirstOrDefaultAsync(s => s.SubjectName.ToUpper() == subjectName.ToUpper()).WithCurrentCulture();
			if (subject != null)
			{
				var courseId = course.Id;
				var subjectId = subject.Id;
				var courseSubject = await _courseSubjects.FirstOrDefaultAsync(cs => cs.CourseId.Equals(courseId) && cs.SubjectId.Equals(subjectId)).WithCurrentCulture();
				if (courseSubject != null)
				{
					_courseSubjects.Remove(courseSubject);
				}
			}
		}

		/// <summary>
		///		Get the names of course's subjects
		/// </summary>
		/// <param name="course"></param>
		/// <returns></returns>
		public virtual IQueryable<string> GetCourseSubjects(TCourse course)
		{
			ThrowIfDisposed();
			if (course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			var query = from courseSubject in _courseSubjects
						where courseSubject.CourseId.Equals(course.Id)
						join subject in _subjectStore.DbEntitySet on courseSubject.SubjectId equals subject.Id
						select subject.SubjectName;
			return query;
		}

		/// <summary>
		///		Returns true if the course has named subject
		/// </summary>
		/// <param name="course"></param>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		public virtual async Task<bool> IsInSubjectAsync(TCourse course, string subjectName)
		{
			ThrowIfDisposed();
			if (course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			if (string.IsNullOrWhiteSpace(subjectName))
			{
				throw new ArgumentException(Resources.Resources.ValueCannotBeNullOrEmpty, nameof(subjectName));
			}
			var subject = await _subjectStore.DbEntitySet.SingleOrDefaultAsync(s => s.SubjectName.ToUpper() == subjectName.ToUpper()).WithCurrentCulture();
			if (subject != null)
			{
				var courseId = course.Id;
				var subjectId = subject.Id;
				return await _courseSubjects.AnyAsync(cs => cs.CourseId.Equals(courseId) && cs.SubjectId.Equals(subjectId)).WithCurrentCulture();
			}
			return false;
		}

		#endregion

		#region ICourseSubjectStore

		/// <summary>
		///		Returns students of the course
		/// </summary>
		/// <returns></returns>
		public virtual IQueryable<TUser> GetStudentsByCourse(TCourse course)
		{
			ThrowIfDisposed();
			if (course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			var query = from userCourse in _userCourses
						where userCourse.CourseId.Equals(course.Id) && userCourse.IsTutor == false
						join user in _userStore.DbEntitySet on userCourse.UserId equals user.Id
						select user;
			return query;
		}

		/// <summary>
		///		Returns tutor of the course
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual TUser GetTutor(TCourse course)
		{
			ThrowIfDisposed();
			if (course is null)
			{
				throw new ArgumentNullException(nameof(course));
			}
			var query = from userCourse in _userCourses
						where userCourse.CourseId.Equals(course.Id) && userCourse.IsTutor == true
						join user in _userStore.DbEntitySet on userCourse.UserId equals user.Id
						select user;
			return query.FirstOrDefault();
		}

		#endregion

		/// <summary>
		///		Dispose the store
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///     If disposing, calls dispose on the Context.  Always nulls out the Context
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (DisposeContext && disposing && Context != null)
			{
				Context.Dispose();
			}
			_disposed = true;
			Context = null;
			_courseStore = null;
		}

		/// <summary>
		///		Throw exception if store is disposed
		/// </summary>
		private void ThrowIfDisposed()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		/// <summary>
		///		Only call save changes  if AutosaveChanges is true
		/// </summary>
		/// <returns></returns>
		private async Task SaveChanges()
		{
			if (AutoSaveChanges)
			{
				await Context.SaveChangesAsync().WithCurrentCulture();
			}
		}

	}
}
