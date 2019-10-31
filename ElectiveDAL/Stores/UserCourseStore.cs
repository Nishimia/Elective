using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces;
using ElectiveDAL.Repositories.AbstractRepositories;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Threading.Tasks;

namespace ElectiveDAL.Stores
{
	/// <summary>
	///		Represent a user course store
	/// </summary>
	/// <typeparam name="TUser"></typeparam>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TUserCourse"></typeparam>
	public class UserCourseStore<TUser, TCourse, TUserCourse> : UserCourseStore<TUser, TCourse, TUserCourse, string>
		where TUser : class, IElectiveUser<string>
		where TCourse : class, ICourse<string>
		where TUserCourse : class, IUserCourse<string>, new()
	{
		public UserCourseStore(DbContext context)
			: base(context)
		{

		}
	}

	/// <summary>
	///		Represent a user course store
	/// </summary>
	/// <typeparam name="TUser"></typeparam>
	/// <typeparam name="TCourse"></typeparam>
	/// <typeparam name="TUserCourse"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public class UserCourseStore<TUser, TCourse, TUserCourse, TKey> : IUserCourseStore<TUser, TCourse, TKey>
		where TUser : class, IElectiveUser<TKey>
		where TCourse : class, ICourse<TKey>
		where TUserCourse : class, IUserCourse<TKey>, new()
		where TKey : IEquatable<TKey>
	{
		private readonly EntityStore<TUser> _userStore;
		private readonly EntityStore<TCourse> _courseStore;
		private readonly IDbSet<TUserCourse> _userCourses;
		private bool _disposed;

		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="context"></param>
		public UserCourseStore(DbContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			_userStore = new EntityStore<TUser>(context);
			_courseStore = new EntityStore<TCourse>(context);
			_userCourses = Context.Set<TUserCourse>();
			DisposeContext = true;
		}

		/// <summary>
		///		If true will call dispose on the DbContext during Dispose
		/// </summary>
		public bool DisposeContext { get; set; }

		/// <summary>
		///		Context for the store
		/// </summary>
		public DbContext Context { get; private set; }

		#region IUserCourseStore

		/// <summary>
		///		Add a user to a course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <param name="isTutor"></param>
		/// <returns></returns>
		public virtual async Task AddToCourseAsync(TUser user, TKey courseId, bool isTutor)
		{
			ThrowIfDisposed();
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var course = await _courseStore.DbEntitySet.FirstOrDefaultAsync(c => c.Id.Equals(courseId)).WithCurrentCulture();
			if (course is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.CourseIdNotFound, courseId));
			}
			var userCourse = new TUserCourse() { CourseId = course.Id, UserId = user.Id, IsTutor = isTutor };
			await Task.FromResult(_userCourses.Add(userCourse)).WithCurrentCulture();
		}

		/// <summary>
		///		Remove a user from the course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task RemoveFromCourseAsync(TUser user, TKey courseId)
		{
			ThrowIfDisposed();
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var userId = user.Id;
			var userCourse = await _userCourses.FirstOrDefaultAsync(uc => uc.UserId.Equals(userId) && uc.CourseId.Equals(courseId)).WithCurrentCulture();
			if (userCourse != null)
			{
				_userCourses.Remove(userCourse);
			}
		}

		/// <summary>
		///		Return all courses user is a member of
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public virtual IQueryable<TCourse> GetUserCourses(TUser user)
		{
			ThrowIfDisposed();
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var userId = user.Id;
			var query = from userCourse in _userCourses
						where userCourse.UserId.Equals(userId)
						join course in _courseStore.DbEntitySet on userCourse.CourseId equals course.Id
						select course;
			return query;
		}

		public virtual IQueryable<TCourse> GetUserCourses(TUser user, bool isTutored)
		{
			ThrowIfDisposed();
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var userId = user.Id;
			var query = from userCourse in _userCourses
						where userCourse.UserId.Equals(userId) && userCourse.IsTutor == isTutored
						join course in _courseStore.DbEntitySet on userCourse.CourseId equals course.Id
						select course;
			return query;
		}

		/// <summary>
		///		Returns true if user is in the course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task<bool> IsInCourseAsync(TUser user, TKey courseId)
		{
			ThrowIfDisposed();
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var userId = user.Id;
			return await _userCourses.AnyAsync(uc => uc.UserId.Equals(userId) && uc.CourseId.Equals(courseId)).WithCurrentCulture();
		}

		/// <summary>
		///		Returns student mark for the course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task<int?> GetMarkAsync(TUser user, TKey courseId)
		{
			if(user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var userCourse = await _userCourses.FirstOrDefaultAsync(uc => uc.CourseId.Equals(courseId) && uc.UserId.Equals(user.Id));
			if(userCourse is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserNotInCourse, nameof(user)));
			}
			var mark = userCourse.StudentMark;
			return mark;
		}

		/// <summary>
		///		Sets user mark for course
		/// </summary>
		/// <param name="user"></param>
		/// <param name="courseId"></param>
		/// <param name="mark"></param>
		/// <returns></returns>
		public virtual async Task SetMarkAsync(TUser user, TKey courseId, int? mark)
		{
			if (user is null)
			{
				throw new ArgumentNullException(nameof(user));
			}
			var userCourse = await _userCourses.FirstOrDefaultAsync(uc => uc.CourseId.Equals(courseId) && uc.UserId.Equals(user.Id));
			if (userCourse is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserNotInCourse, nameof(user)));
			}
			userCourse.StudentMark = mark;
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
	}
}
