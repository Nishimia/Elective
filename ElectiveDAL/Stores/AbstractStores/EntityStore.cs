using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ElectiveDAL.Repositories.AbstractRepositories
{
	/// <summary>
	///		EntityFramework based EntityStore that allows query/manipulation of a TEntity set
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public class EntityStore<TEntity>
		where TEntity : class
	{
		/// <summary>
		///		Constructor that takes a DbContext
		/// </summary>
		/// <param name="dbContext"></param>
		public EntityStore(DbContext context)
		{
			Context = context;
			DbEntitySet = Context.Set<TEntity>();
		}

		/// <summary>
		///		Context	for the store
		/// </summary>
		public DbContext Context { get; private set; }

		/// <summary>
		///		Used to query the entities
		/// </summary>
		public IQueryable<TEntity> EntitySet { get => DbEntitySet; }

		/// <summary>
		///		EntitySet for this store
		/// </summary>
		public DbSet<TEntity> DbEntitySet { get; private set; }

		#region Create/Update/Delete

		/// <summary>
		///		Insert an entity
		/// </summary>
		/// <param name="entity"></param>
		public void Create(TEntity entity) => DbEntitySet.Add(entity);

		/// <summary>
		///		Update an entity
		/// </summary>
		/// <param name="entity"></param>
		public virtual void Update(TEntity entity)
		{
			if (entity != null)
				Context.Entry(entity).State = EntityState.Modified;
		}

		/// <summary>
		///		Mark an entity for deletion
		/// </summary>
		/// <param name="entity"></param>
		public void Delete(TEntity entity) => DbEntitySet.Remove(entity);
		
		#endregion
		
		/// <summary>
		///		FindAsync an entity by ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public virtual Task<TEntity> GetByIdAsync(object id) => DbEntitySet.FindAsync(id);

	}
}
