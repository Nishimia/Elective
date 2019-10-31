using ElectiveBLL.Interfaces.ManagerInterfaces;
using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces;
using ElectiveBLL.Models;
using ElectiveDAL.Services;
using ElectiveDAL.Stores;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Managers
{
	public class ElectiveUserManager : ElectiveUserManager<ElectiveUser, Course, string>
	{
		public ElectiveUserManager(IUserStore<ElectiveUser, string> userStore, IUserCourseStore<ElectiveUser, Course, string> userCourseStore)
			: base(userStore, userCourseStore)
		{

		}

		public static ElectiveUserManager Create(IdentityFactoryOptions<ElectiveUserManager> options, IOwinContext context)
		{
			var manager = new ElectiveUserManager(
				new UserStore<ElectiveUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>(context.Get<DbContext>()),
				new UserCourseStore<ElectiveUser, Course, UserCourse>(context.Get<DbContext>())
				);

			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ElectiveUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = ElectiveBLL.Constants.PASSWORD_MIN_LENGTH,
				RequireNonLetterOrDigit = false,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ElectiveUser>
			{
				MessageFormat = "Your security code is {0}"
			});
			manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ElectiveUser>
			{
				Subject = "Security Code",
				BodyFormat = "Your security code is {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ElectiveUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}

	public class ElectiveUserManager<TUser, TCourse, TKey> : UserManager<TUser, TKey>,
		IUserCourseManager<TUser, TCourse, TKey>
		where TUser : class, IElectiveUser<TKey>
		where TCourse : ICourse<TKey>
		where TKey : IEquatable<TKey>
	{
		private bool _disposed;

		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="store"></param>
		public ElectiveUserManager(IUserStore<TUser, TKey> userStore, IUserCourseStore<TUser, TCourse, TKey> userCourseStore)
			: base(userStore)
		{
			UserCourseStore = userCourseStore ?? throw new ArgumentNullException(nameof(userCourseStore));
		}

		/// <summary>
		///		Persistence abstraction that the ElectiveUserManager operates against
		/// </summary>
		public IUserCourseStore<TUser, TCourse, TKey> UserCourseStore { get; private set; }

		#region IUserCourseManager

		/// <summary>
		///		If is true add to course as tutor, otherwise as student
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="courseId"></param>
		/// <param name="isTutor"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> AddToCourseAsync(TKey userId, TKey courseId, bool isTutor)
		{
			ThrowIfDisposed();
			if (isTutor && !await IsInRoleAsync(userId, ElectiveBLL.Constants.TUTOR_ROLE_NAME).WithCurrentCulture())
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIsNotATutor, userId));
			}
			var user = await FindByIdAsync(userId).WithCurrentCulture();
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			var userCources = UserCourseStore.GetUserCourses(user, isTutor).ToList();
			if (userCources.Any(c => c.Id.Equals(courseId)))
			{
				return new IdentityResult(Resources.Resources.UserAlreadyInCourse);
			}
			await UserCourseStore.AddToCourseAsync(user, courseId, isTutor).WithCurrentCulture();
			return await UpdateAsync(user).WithCurrentCulture();
		}

		/// <summary>
		///		Removes user from course
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> RemoveFromCourseAsync(TKey userId, TKey courseId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId).WithCurrentCulture();
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			if (!await UserCourseStore.IsInCourseAsync(user, courseId))
			{
				return new IdentityResult(Resources.Resources.UserNotInCourse);
			}
			await UserCourseStore.RemoveFromCourseAsync(user, courseId);
			return await UpdateAsync(user).WithCurrentCulture();
		}

		/// <summary>
		///		Returns all studied and tutored courses
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public virtual async Task<IQueryable<TCourse>> GetUserCoursesAsync(TKey userId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId).WithCurrentCulture();
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			return UserCourseStore.GetUserCourses(user);
		}

		public virtual async Task<IQueryable<TCourse>> GetUserCoursesAsync(TKey userId, bool isTutored)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId).WithCurrentCulture();
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			return UserCourseStore.GetUserCourses(user, isTutored);
		}

		/// <summary>
		///		Returns true if user is in course
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task<bool> IsInCourseAsync(TKey userId, TKey courseId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId).WithCurrentCulture();
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			return await UserCourseStore.IsInCourseAsync(user, courseId);
		}

		/// <summary>
		///		Returns student mark for the course
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public virtual async Task<int?> GetMarkAsync(TKey userId, TKey courseId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId);
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			if (!await UserCourseStore.IsInCourseAsync(user, courseId))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserNotInCourse, nameof(user)));
			}
			return await UserCourseStore.GetMarkAsync(user, courseId);
		}

		public virtual async Task<IdentityResult> SetMarkAsync(TKey userId, TKey courseId, int? mark)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId);
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			if (!await UserCourseStore.IsInCourseAsync(user, courseId))
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserNotInCourse, nameof(user)));
			}
			await UserCourseStore.SetMarkAsync(user, courseId, mark);
			return await UpdateAsync(user).WithCurrentCulture();
		}

		/// <summary>
		///		Returns all users in the role
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public virtual List<TUser> GetUsersByRole(string roleName)
		{
			return Users.ToList().Where(u => IsInRoleAsync(u.Id, roleName).Result).ToList();
		}

		/// <summary>
		///		Ban user account
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> BanUserAsync(TKey userId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId);
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			if (user.Banned is true)
			{
				return new IdentityResult(Resources.Resources.UserAlreadyBanned);
			}
			user.Banned = true;
			return await UpdateAsync(user).WithCurrentCulture();
		}

		/// <summary>
		///		Unban user account
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<IdentityResult> UnbanUserAsync(TKey userId)
		{
			ThrowIfDisposed();
			var user = await FindByIdAsync(userId);
			if (user is null)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
					Resources.Resources.UserIdNotFound, userId));
			}
			if (user.Banned is false)
			{
				return new IdentityResult(Resources.Resources.UserAlreadyUnbanned);
			}
			user.Banned = false;
			return await UpdateAsync(user).WithCurrentCulture();
		}

		#endregion

		/// <summary>
		///		Dispose the object
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_disposed = true;
		}

		/// <summary>
		///		Throws an exception when object is disposed
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
