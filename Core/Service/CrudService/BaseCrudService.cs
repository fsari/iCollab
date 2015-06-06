using System;
using System.Data.Entity;
using System.Linq;
using Core.Caching;
using Core.Repository;
using Model;
using PagedList;
using SharpRepository.Repository;

namespace Core.Service.CrudService
{
    public class BaseCrudService<T> : ICrudService<T> where T : BaseEntity, new()
    {
        private readonly IRepository<T> _repository;
        protected readonly ICacheManager Cache; 

        public BaseCrudService
        (
            IRepository<T> repository,
            Func<string, ICacheManager> cache)
        {
            _repository = repository;
            Cache = cache("static"); 
        }

        public virtual T Create(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
             
            _repository.Add(item); 

            Cache.Set(item.Id.ToString(), item); 

            return item;
        }

        public T Find(Guid id)
        {
            var item = _repository.AsQueryable().FirstOrDefault(x=>x.Id == id);

            if (item == null)
            {
                return  default(T);
            } 

            return item;
        }

        public T Update(T item)
        {
            if (item == null)
            {
                return default(T);
            }

            Cache.Remove(item.Id.ToString());
             
            _repository.Update(item); 

            return item;
        }

        public T SoftDelete(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            Cache.Remove(item.Id.ToString());

            item.DateDeleted = DateTime.Now;
            item.IsDeleted = true;
            _repository.Update(item);

            return item;
        }

        public T Delete(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            Cache.Remove(item.Id.ToString());

            item.DateDeleted = DateTime.Now;
            item.IsDeleted = true;
            _repository.Delete(item); 

            return item;
        }

        public IQueryable<T> GetQueryable()
        {
            var items = _repository.AsQueryable().AsNoTracking().Where(x=>x.IsDeleted == false).OrderByDescending(x => x.DateCreated);

            return items;
        } 

        public IPagedList<T> GetPageOf(int pagenumber, int pagesize)
        {
            var items = _repository.AsQueryable().AsNoTracking().Where(x=>x.IsDeleted == false).OrderByDescending(x => x.DateCreated);

            var result = items.ToPagedList(pagenumber, pagesize);

            return result;
        }

        public IQueryable<T> GetTable()
        {
            var table = _repository.AsQueryable().AsNoTracking().Where(i=>i.IsDeleted == false);

            return table;
        }

        public void InvalidateCache(Guid id)
        {
            Cache.Remove(id.ToString());
        }
    }
}
