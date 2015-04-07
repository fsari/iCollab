using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
}