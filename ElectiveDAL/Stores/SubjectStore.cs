using ElectiveBLL.Interfaces.ModelInterfaces;
using ElectiveBLL.Interfaces.StoreInterfaces;
using ElectiveDAL.Repositories.AbstractRepositories;
using System;
using System.Data.Entity;
using System.Data.Entity.Utilities;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Stores
{
	/// <summary>
	///		Represent a subject store
	/// </summary>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TCourseSubject"></typeparam>
	public class SubjectStore<TSubject> : SubjectStore<TSubject, string>
		where TSubject : class, ISubject<string>
	{
		public SubjectStore(DbContext context)
			: base(context)
		{

		}
	}

	/// <summary>
	///		Represent a subject store
	/// </summary>
	/// <typeparam name="TSubject"></typeparam>
	/// <typeparam name="TCourseSubject"></typeparam>
	/// <typeparam name="TKey"></typeparam>
	public class SubjectStore<TSubject, TKey> : 
		ISubjectStore<TSubject, TKey>
		where TSubject : class, ISubject<TKey>
		where TKey : IEquatable<TKey>
	{
		private bool _disposed;
		private EntityStore<TSubject> _subjectStore;

		/// <summary>
		///		Constructor that takes DbContext
		/// </summary>
		/// <param name="context"></param>
		public SubjectStore(DbContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			DisposeContext = true;
			AutoSaveChanges = true;
			_subjectStore = new EntityStore<TSubject>(Context);
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
		///		Returns an IQueryable of subjects
		/// </summary>
		public IQueryable<TSubject> Subjects => _subjectStore.EntitySet;

		#region ISubjectStore

		/// <summary>
		///		Insert a subject 
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public virtual async Task CreateAsync(TSubject subject)
		{
			ThrowIfDisposed();
			if (subject == null)
			{
				throw new ArgumentNullException(nameof(subject));
			}
			_subjectStore.Create(subject);
			await SaveChanges().WithCurrentCulture();
		}

		/// <summary>
		///		Update a subject
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public virtual async Task UpdateAsync(TSubject subject)
		{
			ThrowIfDisposed();
			if (subject == null)
			{
				throw new ArgumentNullException(nameof(subject));
			}
			_subjectStore.Update(subject);
			await SaveChanges().WithCurrentCulture();
		}

		/// <summary>
		///		Mark a subject for deletion
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public virtual async Task DeleteAsync(TSubject subject)
		{
			ThrowIfDisposed();
			if (subject == null)
			{
				throw new ArgumentNullException(nameof(subject));
			}
			_subjectStore.Delete(subject);
			await SaveChanges().WithCurrentCulture();
		}

		/// <summary>
		///		Find subject by ID
		/// </summary>
		/// <param name="subjectId"></param>
		/// <returns></returns>
		public virtual Task<TSubject> FindByIdAsync(TKey subjectId)
		{
			ThrowIfDisposed();
			return _subjectStore.GetByIdAsync(subjectId);
		}

		/// <summary>
		///		Find subjects by name
		/// </summary>
		/// <param name="subjectName"></param>
		/// <returns></returns>
		public virtual async Task<TSubject> FindByNameAsync(string subjectName)
		{
			ThrowIfDisposed();
			if (string.IsNullOrWhiteSpace(subjectName))
			{
				throw new ArgumentException(Resources.Resources.ValueCannotBeNullOrEmpty, nameof(subjectName));
			}
			return await _subjectStore.EntitySet.FirstOrDefaultAsync(c => c.SubjectName.ToUpper() == subjectName.ToUpper()).WithCurrentCulture();
		}

		#endregion

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
			_subjectStore = null;
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
