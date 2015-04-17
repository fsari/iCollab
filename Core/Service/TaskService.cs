using System;
using System.Data.Entity;
using System.Linq;  
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;

namespace Core.Service
{

    public interface ITaskService : ICrudService<Model.Task>
    {
        IQueryable<Task> GetTasksByStatus(TaskStatus status);
        Task GetTask(Guid id, bool nocache = false);
        IQueryable<Task> GetUserTasks(string userId);
        IQueryable<Task> GetTasks();
        IQueryable<Task> GetLateTasks();
        int TasksCount();
        bool UserHasAnyTasks(string userId); 
        IQueryable<Task> Search(string query); 
    } 

    public class TaskService : BaseCrudService<Task>, ITaskService
    {
        private readonly IRepository<Task> _repository;
        private readonly IRepository<TaskUser> _taskUsersRepository; 
        private readonly ICacheManager<Guid, Task> _cache;

        public TaskService(IRepository<Task> repository, ICacheManager<Guid, Task> cache, UoW uow, IRepository<TaskUser> taskUsersRepository)
            : base(repository, cache, uow)
        {
            _repository = repository;
            _cache = cache;
            _taskUsersRepository = taskUsersRepository;
        }

        public IQueryable<Task> GetTasksByStatus(TaskStatus status)
        {
            var tasks = _repository.CollectionUntracked.Where(x => x.TaskStatus == status).OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        private Task GetTaskInstance(Guid id)
        {
            Task task = _repository.Collection
                            .Include(p => p.Project)
                            .Include(a => a.Attachments)
                            .Include(t=>t.SubTasks)
                            .Include(p=>p.ParentTask) 
                            .Include(t=>t.TaskUsers)
                            .FirstOrDefault(x => x.Id == id);

            return task;
        }

        public Task GetTask(Guid id, bool nocache = false)
        {

            Task task;

            if (nocache)
            {
                _cache.InvalidateCacheItem(id);

                task = GetTaskInstance(id);
                  
                return task;
            }

            task = _cache.Get(id);

            if (task == null)
            {
                task = GetTaskInstance(id);

                if (task == null)
                {
                    return null;
                }

                _cache.Set(id, task);
            }

            return task;
        }

        public IQueryable<Task> GetUserTasks(string userId)
        {
            /*var tasks = _repository.CollectionUntracked
                            .Include(p => p.Project)  
                            .Include(a => a.Attachments)
                            .Where(x => x.TaskOwner == username)
                            .OrderByDescending(t => t.DateCreated);*/

            var userTasks = _repository.CollectionUntracked.Include(t => t.TaskUsers).Where(x => x.TaskUsers.Any(r => r.UserId == userId));

            return userTasks;
        }

        public IQueryable<Task> GetTasks()
        {
            var tasks = _repository.CollectionUntracked.OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        public IQueryable<Task> GetLateTasks()
        {
            var tasks = _repository.CollectionUntracked.Where(x => x.End < DateTime.Now && x.TaskStatus != TaskStatus.Tamamlandı).OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        public int TasksCount()
        {
            var count = _repository.CollectionUntracked.Count();

            return count;
        }

        public bool UserHasAnyTasks(string userId)
        {
            return false; //_repository.CollectionUntracked.Any(x => x.TaskOwner == userId);
        }

        public IQueryable<Task> Search(string query)
        {
            var tasks = _repository.CollectionUntracked.Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(t => t.DateCreated);

            return tasks;
        }
         
    }
}
