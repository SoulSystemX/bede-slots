using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace bede_slots.Domain
{

    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);

        Task<bool> Delete(object id);
        Task<bool> Delete(T entity);
        Task<bool> Delete(Expression<Func<T, bool>> where);

        Task<T> Get(object id);
        Task<T> Get(Expression<Func<T, bool>> where);

        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
        IEnumerable<T> GetAll();

        Task<int> Count(Expression<Func<T, bool>> where);
        Task<int> Count();

        IQueryable<T> Table { get; }
        IQueryable<T> TableNoTracking { get; }

    }
    internal class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private readonly IAppDbContext _appDbContext;

        protected DbSet<TEntity> _entities;

        public Repository(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // If an error has occured when trying to alter an entity rollback the changes
        protected string GetErrorAndRollBackEntityChanges(DbUpdateException exception)
        {
            if(_appDbContext is DbContext dbContext )
            {
                var entries = dbContext.ChangeTracker
                    .Entries()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                    .ToList();

                entries.ForEach(entry =>
                {
                    try
                    {
                        entry.State = EntityState.Unchanged;
                    }
                    catch(InvalidOperationException)
                    {
                        //Log to logger
                    }
                }
                );
            }

            try
            {
                _appDbContext.SaveChanges();
                return exception.ToString();
            }
            catch(Exception ex)
            {
                //if the rollback has failed and still can't save to the context, return the full error message
                return ex.ToString();
            }


        }
        
        protected virtual DbSet<TEntity> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _appDbContext.Set<TEntity>();

                return _entities;
            }
        }

        public virtual void Add(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.AddAsync(entity);

            }
            catch (Exception) { throw;  }
        }


        public virtual void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Update(entity);

            }
            catch (Exception) { throw; }
        }

        public virtual async Task<bool> Delete(object id)
        {
            try
            {
                var entity = await Entities.FindAsync(id);
                if (entity == null)
                    return false;

                Entities.Remove(entity);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                    return false;

                Entities.Remove(entity);
                    return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual async Task<bool> Delete(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                var entities = Entities.Where(where);
                Entities.RemoveRange(entities);
                return true;

            }
            catch (DbUpdateException exception)
            {
                throw new Exception(GetErrorAndRollBackEntityChanges(exception), exception);
            }
        }

        public virtual async Task<TEntity> Get(object id)
        {
            try
            {
                return await Entities.FindAsync(id);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public virtual async Task<TEntity> Get(Expression<Func<TEntity, bool>> where)
        {
            try
            {
                return await Entities.FirstOrDefaultAsync(where);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            try
            {
                return Entities;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IEnumerable<TEntity> GetMany(Expression<Func<TEntity, bool>> where)
        {
            return Entities.Where(where);
        }

        public virtual void Save()
        {
            _appDbContext.SaveChanges();
        }

        public virtual void SaveAsync()
        {
            _appDbContext.SaveChangesAsync();
        }
        public virtual async Task<int> Count()
        {
            return await Entities.CountAsync();
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> where)
        {
            return await Entities.CountAsync(where);
        }

        public virtual IQueryable<TEntity> Table => Entities;

        public virtual IQueryable<TEntity> TableNoTracking => Entities.AsNoTracking();

    }
}
