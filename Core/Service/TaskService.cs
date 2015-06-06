using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;  
using Core.Caching;
using Core.Extensions;
using Core.Repository;
using Core.Service.CrudService;
using Kendo.Mvc.Extensions;
using Microsoft.AspNet.Identity;
using Model;
using SharpRepository.Repository;

namespace Core.Service
{

    public interface ITaskService : ICrudService<Model.Task>
    {
        IQueryable<Task> GetTasksByStatus(TaskStatus status);
        Task GetTask(Guid id, bool nocache = false);
        IQueryable<Task> GetUserTasks(string userId);
        IQueryable<Task> GetTasks();
        IQueryable<Task> GetLateTasks(); 
        IQueryable<Task> GetTasksUserCreated(string userId);  
        int TasksCount();
        bool UserHasAnyTasks(string userId); 
        IQueryable<Task> Search(string query); 
        Task GetNext(Task task);
        Task GetPrevious(Task task); 
        IQueryable<Task> SearchUserTaks(string query, string userId);
        bool TaskUser(string userId, Guid id);
    } 

    public class TaskService : BaseCrudService<Task>, ITaskService
    {
        private readonly IRepository<Task> _repository; 
        private readonly ICacheManager _cache; 

        public TaskService(IRepository<Task> repository, Func<string, ICacheManager> cache)
            : base(repository, cache)
        {
            _repository = repository;
            _cache = cache("static"); 
        } 

        public IQueryable<Task> GetTasksByStatus(TaskStatus status)
        {
            var tasks = _repository.AsQueryable().Where(x => x.TaskStatus == status).OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        private Task GetTaskInstance(Guid id)
        {
            Task task = _repository.AsQueryable()
                            .Include(p => p.Project)
                            .Include(a => a.Attachments)
                            .Include(t=>t.SubTasks)
                            .Include(p=>p.ParentTask) 
                            .Include(t=>t.TaskUsers)
                            .Include(t=>t.TaskOwner)
                            .FirstOrDefault(x => x.Id == id);

            return task;
        }

        public Task GetTask(Guid id, bool nocache = false)
        {

            Task task;

            if (nocache)
            {
                _cache.Remove(id.ToString());

                task = GetTaskInstance(id);
                  
                return task;
            }

            task = _cache.Get(id.ToString(), () => GetTaskInstance(id));

            /*if (task == null)
            {
                task = GetTaskInstance(id);

                if (task == null)
                {
                    return null;
                }

                _cache.Set(id, task);
            }*/

            return task;
        }

        public IQueryable<Task> GetUserTasks(string userId)
        {
            var userTasks = _repository.AsQueryable().Include(t => t.TaskUsers).Where(x => x.TaskUsers.Any(r => r.UserId == userId)).OrderByDescending(x=>x.DateCreated);

            return userTasks;
        }

        public IQueryable<Task> GetTasks()
        {
            var tasks = _repository.AsQueryable().OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        public IQueryable<Task> GetLateTasks()
        {
            var tasks = _repository.AsQueryable().Where(x => x.End < DateTime.Now && x.TaskStatus != TaskStatus.Tamamlandı).OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        public IQueryable<Task> GetTasksUserCreated(string userId)
        {
            var tasks = _repository.AsQueryable().Where(x => x.TaskOwnerId == userId).OrderByDescending(x => x.DateCreated);

            return tasks;
        }

        public int TasksCount()
        {
            var count = _repository.AsQueryable().Count();

            return count;
        }

        public bool UserHasAnyTasks(string userId)
        {
            var result = _repository.AsQueryable().Any(x => x.TaskUsers.Any(u => u.UserId == userId));
            //CHECK THIS if user has any tasks
            return result;
        }

        public IQueryable<Task> Search(string query)
        {
            var tasks = _repository.AsQueryable().Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(t => t.DateCreated);

            return tasks;
        }

        public Task GetNext(Task task)
        {
            var next = _repository.AsQueryable().GetNext(task);

            return next;
        }

        public Task GetPrevious(Task task)
        {
            var previous = _repository.AsQueryable().GetPrevious(task);

            return previous;
        }

        public IQueryable<Task> SearchUserTaks(string query, string userId)
        {
            var tasks = _repository.AsQueryable().Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(t => t.DateCreated);
            return tasks;
        }

        public bool TaskUser(string userId, Guid id)
        {
            var task = GetTask(id, true);

            if (task.TaskOwnerId == userId)
            {
                return true;
            }

            if (task.TaskUsers.Any(x => x.UserId == userId))
            {
                return true;
            }

            return false;
        }
    }
}
