using System; 
using System.Linq;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;
using Model.Activity;

namespace Core.Service
{

    public interface IActivityService<T> : ICrudService<T> where T : Activity 
    {

    }

    public class ActivityService<T> : BaseCrudService<T>, IActivityService<T> where T: Activity, new()
    {
        private readonly IRepository<T> _repository; 

        public ActivityService(IRepository<T> repository, ICacheManager<Guid, T> cache, UoW uow)
            : base(repository, cache, uow)
        {
            _repository = repository;
        }
    }
}
