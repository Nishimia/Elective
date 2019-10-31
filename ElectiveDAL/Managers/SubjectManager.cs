using ElectiveBLL.Interfaces.ManagerInterfaces;
using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces;
using ElectiveBLL.Models;
using ElectiveDAL.Stores;
using ElectiveDAL.Validators;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Managers
{
	public class SubjectManager : SubjectManager<Subject, string>
	{
		public SubjectManager(ISubjectStore<Subject, string> store)
			: base(store)
		{

		}

		public static SubjectManager Create(IdentityFactoryOptions<SubjectManager> options, IOwinContext context)
		{
			return new SubjectManager(new SubjectStore<Subject>(context.Get<DbContext>()));
		}
	}

	public class SubjectManager<TSubject, TKey> : ISubjectManager<TSubject, TKey>
		where TSubject : class, ISubject<TKey>
		where TKey : IEquatable<TKey>
	{
		private IIdentityValidator<TSubject> _subjectValidator;
		private bool _disposed;

		/// <summary>
		///		Constructor
		/// </summary>
		/// <param name="store"></param>
		public SubjectManager(ISubjectStore<TSubject, TKey> store)
		{
			Store = store ?? throw new ArgumentNullException(nameof(store));
			SubjectValidator = new SubjectValidator<TSubject, TKey>(this);
		}

		/// <summary>
		///		Persistence abstraction that the SubjectManager operates against
		/// </summary>
		public ISubjectStore<TSubject, TKey> Store { get; private set; }

		#region ISubjectManager

		/// <summary>
		///		Used to validate subjects before changes are saved
		/// </summary>
		public virtual IIdentityValidator<TSubject> SubjectValidator
		{
			get
			{
				ThrowIfDisposed();
				return _subjectValidator;
			}

			set
			{
				ThrowIfDisposed();
				if (value is null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				_subjectValidator = value;
			}
		}

		/// <summary>
		///		Returns IQueryable of subjects if the store is an IQueryableSubjectStore
		/// </summary>
		public virtual IQueryable<TSubject> Subjects
		{
			get
			{
				return Store.Subjects;
			}
		}

		/// <summary>
		///		Create a subject
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> CreateAsync(TSubject subject)
		{
			ThrowIfDisposed();
			var result = await SubjectValidator.ValidateAsync(subject).WithCurrentCulture();
			if (!result.Succeeded)
			{
				return result;
			}
			await Store.CreateAsync(subject).WithCurrentCulture();
			return IdentityResult.Success;
		}

		/// <summary>
		///		Delete a subject
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> DeleteAsync(TSubject subject)
		{
			ThrowIfDisposed();
			await Store.DeleteAsync(subject).WithCurrentCulture();
			return IdentityResult.Success;
		}

		/// <summary>
		///		Find subject by id
		/// </summary>
		/// <param name="subjectId"></param>
		/// <returns></returns>
		public virtual Task<TSubject> FindByIdAsync(TKey subjectId)
		{
			ThrowIfDisposed();
			return Store.FindByIdAsync(subjectId);
		}

		/// <summary>
		///		Find subject by name
		/// </summary>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		public virtual async Task<TSubject> FindByNameAsync(string subjectName)
		{
			ThrowIfDisposed();
			if (subjectName is null)
			{
				throw new ArgumentNullException(nameof(subjectName));
			}
			return await Store.FindByNameAsync(subjectName);
		}

		/// <summary>
		///		Update a subject
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public virtual async Task<IdentityResult> UpdateAsync(TSubject subject)
		{
			ThrowIfDisposed();
			if (subject is null)
			{
				throw new ArgumentNullException(nameof(subject));
			}
			var result = await SubjectValidator.ValidateAsync(subject).WithCurrentCulture();
			if (!result.Succeeded)
			{
				return result;
			}
			await Store.UpdateAsync(subject).WithCurrentCulture();
			return IdentityResult.Success;
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
