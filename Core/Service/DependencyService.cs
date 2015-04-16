using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;
using Task = Model.Task;
using TaskStatus = Model.TaskStatus;

namespace Core.Service
{

    public interface IDependencyService : ICrudService<Dependency>
    {
/*        IQueryable<Dependency> GetDependencies();
        void DeleteDependency(Dependency entity);
        void CreateDependency(Dependency entity);
        void UpdateDependency(Dependency entity);*/
    } 

    public class DependencyService : BaseCrudService<Dependency>, IDependencyService
    {
        private readonly IRepository<Dependency> _repository;

        public DependencyService(
            IRepository<Dependency> repository, 
            ICacheManager<Guid, Dependency> cache,
            UoW uow)
            : base(repository, cache, uow)
        {
            this._repository = repository;
        }

      /*  public IQueryable<Dependency> GetDependencies()
        {
            var dependencies = _repository.CollectionUntracked;

            return _repository.;
        }

        public void DeleteDependency(Dependency entity)
        {
            throw new NotImplementedException();
        }

        public void CreateDependency(Dependency entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateDependency(Dependency entity)
        {
            throw new NotImplementedException();
        }*/
    }
}
