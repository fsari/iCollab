using System;
using System.Linq;
using Core.Caching;
using Core.Repository;
using Model;
using PagedList;

namespace Core.Service.CrudService
{
    public class BaseCrudService<T> : ICrudService<T> where T : BaseEntity, new()
    {
        private readonly IRepository<T> _repository;
        protected readonly ICacheManager Cache;
        private readonly UoW _uow;

        public BaseCrudService
        (
            IRepository<T> repository,
            Func<string, ICacheManager> cache, 
            UoW uow)
        {
            _repository = repository;
            Cache = cache("static");
            _uow = uow;
        }

        public T Create(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }
             
            var instance = _repository.Add(item);

            Cache.Set(item.Id.ToString(), item);

            _uow.Commit();

            return instance;
        }

        public T Find(Guid id)
        {
            var item = _repository.Find(id);

            if (item == null)
            {
                return  default(T);
            }

            _uow.Commit();

            return item;
        }

        public T Update(T item)
        {
            if (item == null)
            {
                return default(T);
            }

            Cache.Remove(item.Id.ToString());

            item.DateEdited = DateTime.Now;
            var instance = _repository.Update(item);

            var result = _uow.Commit();

            return instance;
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
            var instance = _repository.Update(item);

            _uow.Commit();

            return instance;
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
            var instance = _repository.Delete(item);

            _uow.Commit();

            return instance;
        }

        public IQueryable<T> GetQueryable()
        {
            var items = _repository.CollectionUntracked.Where(x=>x.IsDeleted == false).OrderByDescending(x => x.DateCreated);

            return items;
        } 

        public IPagedList<T> GetPageOf(int pagenumber, int pagesize)
        {
            var items = _repository.CollectionUntracked.Where(x=>x.IsDeleted == false).OrderByDescending(x => x.DateCreated);

            var result = items.ToPagedList(pagenumber, pagesize);

            return result;
        }

        public IQueryable<T> GetTable()
        {
            var table = _repository.CollectionUntracked.Where(i=>i.IsDeleted == false);

            return table;
        }

        public void InvalidateCache(Guid id)
        {
            Cache.Remove(id.ToString());
        }
    }
}
