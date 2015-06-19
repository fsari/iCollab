using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq; 
using Core.Caching;
using Core.Extensions; 
using Core.Service.CrudService;
using Model;
using SharpRepository.Repository;

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
        IQueryable<Project> GetUserProjects(ApplicationUser user); 
        ICollection<Task> GetProjectTasks(Guid id);  
        IQueryable<Project> Search(string query);
        IQueryable<Project> SearchUserProjects(string query, string userId);
        bool ProjectUser(string userId, Guid id);
        IEnumerable<ProjectUsers> GetProjectUsers(Guid projectId);
    }

    public class ProjectService : BaseCrudService<Project>, IProjectService
    {
        private readonly IRepository<Project> _repository; 

        public ProjectService(IRepository<Project> repository, Func<string, ICacheManager> cache) : base(repository, cache)
        { 
            _repository = repository;
        }

        public IEnumerable<ProjectUsers> GetProjectUsers(Guid projectId)
        {
            var project = _repository.AsQueryable().Include(u => u.ProjectUsers).FirstOrDefault(x => x.Id == projectId);

            if (project == null)
            {
                return null;

            } 
            return project.ProjectUsers.AsEnumerable(); 
        }
         
        public IQueryable<Project> GetProjects()
        {
            var projects = _repository.AsQueryable().OrderByDescending(o => o.DateCreated);

            return projects;
        }

        private Project GetProjectInstance(Guid id)
        {
            Project project = _repository.AsQueryable(). 
                                        Include(a => a.Attachments).
                                        Include(d => d.Documents).
                                        Include(t => t.Tasks).
                                        Include(m=>m.Meetings). 
                                        Include(u=>u.ProjectUsers).
                                        Include(p=>p.ProjectOwner).
                                        FirstOrDefault(m => m.Id == id);

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

            project = Cache.Get(id.ToString(), () => GetProjectInstance(id));
             
            return project;
        }

        public IQueryable<Project> GetProjectsByStatus(ProjectStatus projectStatus)
        {
            var projects = _repository.AsQueryable() 
                                      .Where(x=>x.IsDeleted == false)
                                      .Where(x => x.Status == projectStatus)
                                      .OrderByDescending(o => o.DateCreated);

            return projects;
        }

        public IQueryable<Project> GetLateProjects()
        {
            var projects = _repository.AsQueryable() 
                                      .Where(x=>x.IsDeleted == false)
                                      .Where(x => x.EndDatetime < DateTime.Now && x.Status != ProjectStatus.Bitti)
                                      .OrderByDescending(o => o.DateCreated);

            return projects;
        }

        public int ProjectsCount()
        {
            int count = _repository.AsQueryable().Count(x=>x.IsDeleted == false);

            return count;
        }

        public Project GetNextProject(Project project)
        {
            var nextProject = _repository.AsQueryable().GetNext(project);

            return nextProject;
        }

        public Project GetPreviousProject(Project project)
        {
            var previousProject = _repository.AsQueryable().GetPrevious(project);

            return previousProject;
        }

        public IQueryable<Project> GetUserProjects(ApplicationUser user)
        {  
            var projects = _repository.AsQueryable().Include(u=>u.ProjectOwner)
                                      .Include(p=>p.ProjectUsers).Include(o=>o.ProjectOwner)
                                      .Where(x=>x.IsDeleted == false)
                                      .Where(x => x.ProjectUsers.Any(e=>e.UserId == user.Id) || x.ProjectOwnerId == user.Id)
                                      .OrderByDescending(x => x.DateCreated);

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
            var project = _repository.AsQueryable().Include(t => t.Tasks).FirstOrDefault(i=>i.Id == id);

            if (project == null)
            {
                return null;
            }

            return project.Tasks;
        }
  
        public IQueryable<Project> Search(string query)
        {
            var projects = _repository.AsQueryable() 
                                      .Where(x=>x.IsDeleted == false)
                                      .Where(x => x.Title.Contains(query) || x.Description.Contains(query))
                                      .OrderByDescending(x=>x.DateCreated);

            return projects;
        }

        public IQueryable<Project> SearchUserProjects(string query, string userId)
        {
            var projects = _repository.AsQueryable()
                                      .Include(u => u.ProjectOwner)
                                      .Include(p => p.ProjectUsers)
                                      .Where(x=>x.IsDeleted == false)
                                      .Where(x => x.ProjectUsers.Any(e => e.UserId == userId) || x.ProjectOwnerId == userId)
                                      .Where(x => x.Title.Contains(query) || x.Description.Contains(query))
                                      .OrderByDescending(x => x.DateCreated);

            return projects;
        }
    }
}
