using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Core.Caching;
using Core.Infrastructure;
using Core.Repository;
using Microsoft.AspNet.Identity;
using Model;
using PagedList;

namespace Core.Service
{

    public interface IUserService
    {
        ApplicationUser GetCurrentUser(string username); 
        ApplicationUser FindById(string userId);
        ApplicationUser FindByEmail(string email);
        bool IsUserManager(string userId);
        ApplicationUser Delete(ApplicationUser user);
        void AssignManager(string userId);
        IQueryable<ApplicationUser> GetAllUsers();
        IPagedList<ApplicationUser> GetPageOf(int pagenumber, int pagesize); 
        IEnumerable<SelectListItem> GetUsersDropDown();
        int GetUserCount();
        IEnumerable<string> GetOnlineUsers();
        IQueryable<ApplicationUser> GetUsers(IEnumerable<string> userId); 
        bool ChangePassword(string userId, string currentPassword, string newPassword); 
        IEnumerable<ProjectUsers> GetProjectUsers(Guid projectId);
        bool Update(ApplicationUser user);

        ApplicationUser FindByUsername(string userName);
    }

    public class UserService : IUserService
    {
        private readonly UoW _uow;
        private readonly ApplicationUserManager _userManager; 
        private readonly ICacheManager _userCache; 

        public UserService(
                           UoW uow,
                           ApplicationUserManager usermanager,
                           Func<string, ICacheManager> cache)
        {
            _userCache = cache("static");
            _uow = uow;
            _userManager = usermanager;  
        }

        public bool ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var result = _userManager.ChangePassword(userId, currentPassword, newPassword);

            if (result.Succeeded)
            {
                return true;
            }

            return false;
        } 

        public IEnumerable<ProjectUsers> GetProjectUsers(Guid projectId)
        { 
            var project = _uow.Context.Set<Project>().Include(u=>u.ProjectUsers).FirstOrDefault(x => x.Id == projectId);

            if (project == null)
            {
                return null;

            }

            return project.ProjectUsers.AsEnumerable();

        }

        public void AssignManager(string userId)
        { 
            var result = _userManager.AddToRole(userId, "manager");
        }

        public IEnumerable<SelectListItem> GetUsersDropDown()
        {
            IEnumerable<ApplicationUser> users = GetAllUsers();

            return users.Select(x => new SelectListItem { Text = x.UserName, Value = x.Id });
        }

        public int GetUserCount()
        {
            var userCount = _uow.Context.Set<ApplicationUser>().Count();

            return userCount;
        } 

        public bool IsUserManager(string userId)
        {
            return _userManager.IsInRole(userId, "manager");
        }
         
        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return _uow.Context.Set<ApplicationUser>();
        }

        public IEnumerable<string> GetOnlineUsers()
        {
            var tenminutesbefore = DateTime.Now.Subtract(TimeSpan.FromMinutes(10));
            
            var users =
                _uow.Context.Set<ApplicationUser>()
                    .Where(x => x.LastLogin > tenminutesbefore)
                    .Select(e => e.UserName);

            return users;
        }

        public IQueryable<ApplicationUser> GetUsers(IEnumerable<string> userId)
        {
            var users = _uow.Context.Set<ApplicationUser>().AsNoTracking().Where(x => userId.Contains(x.Id));

            return users;
        }

        public ApplicationUser FindByUsername(string userName)
        {
            var user = _uow.Context.Set<ApplicationUser>().Include(p=>p.Picture).FirstOrDefault(x => x.UserName == userName);

            return user;
        }

        public ApplicationUser FindByEmail(string email)
        {
            var user = _userManager.FindByEmail(email);

            return user;
        } 

        public ApplicationUser FindById(string userId)
        {
            var user = _uow.Context.Set<ApplicationUser>().FirstOrDefault(x => x.Id == userId);

            return user;
        }

        public ApplicationUser GetCurrentUser(string userName)
        {
            ApplicationUser appUser = _userCache.Get(userName, () => GetUserInstance(userName));
              
            return appUser;
        }

        public ApplicationUser GetUserInstance(string userName)
        {
            ApplicationUser user = _uow.Context.Set<ApplicationUser>().Include(a => a.Picture).AsNoTracking().FirstOrDefault(x => x.UserName == userName);
             
            return user;
        }


        public ApplicationUser Create(ApplicationUser user)
        {
            var result = _userManager.Create(user);

            if (result.Succeeded)
            {
                return user;
            }
           
            return null;
        } 

        public bool Update(ApplicationUser user)
        { 
            var result = _userManager.Update(user);

            if (result.Succeeded)
            {
                _userCache.Remove(user.Email);

                return true;
            }

            return true;
        }
        
        public ApplicationUser Delete(ApplicationUser item)
        {
            var user = _uow.Context.Set<ApplicationUser>().Remove(item);

            return user;
        }

        public IQueryable<ApplicationUser> GetQueryable()
        {
            var users = _uow.Context.Set<ApplicationUser>().AsNoTracking();
            return users;
        }

        public IPagedList<ApplicationUser> GetPageOf(int pagenumber, int pagesize)
        {
            var users = _uow.Context.Set<ApplicationUser>().Include(p=>p.Picture).AsNoTracking().OrderByDescending(x => x.Id).ToPagedList(pagenumber, pagesize);

            return users;
        }  
    }
}
