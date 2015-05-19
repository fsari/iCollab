using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EntityFramework.BulkInsert.Extensions;
using Model;

namespace Core.Repository
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> CollectionUntracked { get; }
        IQueryable<TEntity> Collection { get; }
        void Add(IEnumerable<TEntity> entities);
        TEntity Add(TEntity entity);
        TEntity Update(TEntity entity);
        TEntity Delete(TEntity entity);
        void Delete(object id);
        TEntity Find(object id);
        Task<TEntity> FindAsync(object id);
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);

    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {

        private readonly UoW _uow;

        public Repository(UoW uow)
        { 
            _uow = uow;
        }

        public IQueryable<TEntity> CollectionUntracked
        {
            get { return _uow.Context.Set<TEntity>().AsNoTracking().Where(i=>i.IsDeleted == false); }
        }

        public IQueryable<TEntity> Collection
        {
            get { return _uow.Context.Set<TEntity>().Where(i => i.IsDeleted == false); }
        }

        public void Add(IEnumerable<TEntity> entities)
        {
            var options = new BulkInsertOptions
            {
                EnableStreaming = true,
            };

            _uow.Context.BulkInsert(entities, options);
        } 

        public TEntity Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            _uow.Context.Set<TEntity>().Attach(entity);

            var instance = _uow.Context.Set<TEntity>().Add(entity);

            return instance;
        }

        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            _uow.Context.Set<TEntity>().Attach(entity);

            _uow.Context.Entry(entity).State = EntityState.Modified;

            return entity;
        }

        public TEntity Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            var entry = _uow.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _uow.Context.Set<TEntity>().Attach(entity);
            }
             
            _uow.Context.Set<TEntity>().Remove(entity);

            return entity;
        }

        public void Delete(object id)
        {
            var entity = Find(id);

            if (entity == null)
            {
                return;
            }

            _uow.Context.Set<TEntity>().Remove(entity);
        }

        public TEntity Find(object id)
        {
            var instance = _uow.Context.Set<TEntity>().Find(id);

            return instance;
        }

        public async Task<TEntity> FindAsync(object id)
        {
            var instance = await _uow.Context.Set<TEntity>().FindAsync(id).ConfigureAwait(false);

            return instance;
        } 

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = _uow.Context.Set<TEntity>().Where(predicate);
            return query;
        }
    }
}