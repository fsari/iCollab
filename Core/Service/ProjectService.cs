using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Core.Caching;
using Core.Extensions;
using Core.Repository;
using Core.Service.CrudService;
using Model;

namespace Core.Service
{

    public interface IProjectService : ICrudService<Project>
    {
        IQueryable<Project> GetProjects();
        Project GetProject(Guid id, bool nocache = false);
        IQueryable<Project> GetProjectsByStatus(ProjectStatus projectStatus);
        IQueryable<Project> GetLateProjects();
        int ProjectsCount();
        Project GetNextProject(Project project);
        Project GetPreviousProject(Project project);
        IQueryable<Project> GetUserProjects(string username); 
        ICollection<Task> GetProjectTasks(Guid id);  
        IQueryable<Project> Search(string query);
        IQueryable<Project> SearchUserProjects(string query, string userId);
        bool ProjectUser(string userId, Guid id);
    }

    public class ProjectService : BaseCrudService<Project>, IProjectService
    {
        private readonly IRepository<Project> _repository;
        private readonly ICacheManager<Guid, Project> _cache; 

        public ProjectService(
            IRepository<Project> repository, 
            ICacheManager<Guid, Project> cache, 
            UoW uow
            )
            : base(repository, cache, uow)
        {
            _cache = cache; 
            _repository = repository;
        }
         
        public IQueryable<Project> GetProjects()
        {
            var projects = _repository.CollectionUntracked.OrderByDescending(o => o.DateCreated);

            return projects;
        }

        private Project GetProjectInstance(Guid id)
        {
            Project project = _repository.Collection. 
                    Include(a => a.Attachments).
                    Include(d => d.Documents).
                    Include(t => t.Tasks).
                    Include(m=>m.Meetings). 
                    Include(u=>u.ProjectUsers).
                    Include(p=>p.ProjectOwner).
                    FirstOrDefault(m => m.Id == id);

            if (project == null)
            {
                return null;
            }

            return project;
        }

        public Project GetProject(Guid id, bool nocache = false)
        {
            Project project;

            if (nocache)
            {
                project = GetProjectInstance(id);

                return project;
            }
            
            project = _cache.Get(id);

            if (project == null)
            {
                project = GetProjectInstance(id);

                if (project == null)
                {
                    return null;
                }

                _cache.Set(id, project);
            }

            return project;
        }

        public IQueryable<Project> GetProjectsByStatus(ProjectStatus projectStatus)
        {
            var projects = _repository.CollectionUntracked 
                .Where(x => x.Status == projectStatus)
                .OrderByDescending(o => o.DateCreated);

            return projects;
        }

        public IQueryable<Project> GetLateProjects()
        {
            var projects = _repository.CollectionUntracked 
                .Where(x => x.EndDatetime < DateTime.Now && x.Status != ProjectStatus.Bitti)
                .OrderByDescending(o => o.DateCreated);

            return projects;
        }

        public int ProjectsCount()
        {
            int count = _repository.CollectionUntracked.Count();

            return count;
        }

        public Project GetNextProject(Project project)
        {
            var nextProject = _repository.CollectionUntracked.GetNext(project);

            return nextProject;
        }

        public Project GetPreviousProject(Project project)
        {
            var previousProject = _repository.CollectionUntracked.GetPrevious(project);

            return previousProject;
        }

        public IQueryable<Project> GetUserProjects(string userId)
        {  
            var projects = _repository.CollectionUntracked.Include(t=>t.Tasks).Include(u=>u.ProjectOwner).Include(p=>p.ProjectUsers).Where(x => x.ProjectUsers.Any(e=>e.UserId == userId) || x.ProjectOwnerId == userId).OrderByDescending(x => x.DateCreated);

            return projects;
        }

        public bool ProjectUser(string userId, Guid id)
        {
            var project = GetProject(id, true);

            if (project.ProjectOwnerId == userId)
            {
                return true;
            }

            if (project.ProjectUsers.Any(x => x.UserId == userId))
            {
                return true;
            }

            return false;
        }
  
        public ICollection<Task> GetProjectTasks(Guid id)
        {
            var project = _repository.CollectionUntracked.Include(t => t.Tasks).FirstOrDefault(i=>i.Id == id);

            if (project == null)
            {
                return null;
            }

            return project.Tasks;
        }
  
        public IQueryable<Project> Search(string query)
        {
            var projects = _repository
                .CollectionUntracked 
                .Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(x=>x.DateCreated);

            return projects;
        }

        public IQueryable<Project> SearchUserProjects(string query, string userId)
        {
            var projects = _repository.CollectionUntracked.Include(u => u.ProjectOwner).Include(p => p.ProjectUsers)
                .Where(x => x.ProjectUsers.Any(e => e.UserId == userId) || x.ProjectOwnerId == userId)
                .Where(x => x.Title.Contains(query) || x.Description.Contains(query))
                .OrderByDescending(x => x.DateCreated);

            return projects;
        }
    }
}
