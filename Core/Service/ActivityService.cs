using System; 
using System.Linq;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;
using Model.Activity;

namespace Core.Service
{

    public interface IActivityService : ICrudService<Activity>
    {

    }

    public class ActivityService : BaseCrudService<Activity>, IActivityService
    {
        public ActivityService(IRepository<Activity> repository, ICacheManager<Guid, Activity> cache, UoW uow) : base(repository, cache, uow)
        {
        }
    }
}
