using System;
using System.Linq;
using PagedList;

namespace Core.Service.CrudService
{
    public interface ICrudService<T>
    {

        T Create(T item);
        T Find(Guid id);
        T Update(T item);
        T SoftDelete(T item);
        T Delete(T item);
        IQueryable<T> GetQueryable();
        IPagedList<T> GetPageOf(int pagenumber, int pagesize);
        IQueryable<T> GetTable();

        void InvalidateCache(Guid id);

    }
}
