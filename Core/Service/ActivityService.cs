using System; 
using System.Linq;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;
using Model.Activity;

namespace Core.Service
{

    public interface IActivityService : ICrudService<ProjectActivity>
    {

    }

    public class ActivityService : BaseCrudService<ProjectActivity>, IActivityService
    {
        public ActivityService(IRepository<ProjectActivity> repository, ICacheManager<Guid, ProjectActivity> cache, UoW uow)
            : base(repository, cache, uow)
        {
        }
    }
}
