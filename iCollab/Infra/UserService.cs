﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using Core.Caching;
using Core.Repository;
using iCollab.ViewModels;
using Microsoft.AspNet.Identity;
using Model;
using PagedList;

namespace iCollab.Infra
{

    public interface IUserService
    {
        AppUserViewModel GetCurrentUser(string username);
        ApplicationUser GetUserInstance(string userName);
        ApplicationUser Find(string userId);
        bool IsUserManager(string userId);
        ApplicationUser Delete(ApplicationUser user);
        void AssignManager(string userId);
        IQueryable<ApplicationUser> GetAllUsers();
        IPagedList<ApplicationUser> GetPageOf(int pagenumber, int pagesize);
        IQueryable<ApplicationUser> GetTable();
        IEnumerable<SelectListItem> GetUsersDropDown();
        int GetUserCount();
        IEnumerable<string> GetOnlineUsers();
        IEnumerable<ApplicationUser> GetUsers(List<string> userId);
    }

    public class UserService : IUserService
    {
        private readonly UoW _uow;
        private readonly ApplicationUserManager _userManager; 
        private readonly ICacheManager<string, AppUserViewModel> _userCache; 

        public UserService(
                           UoW uow,
                           ApplicationUserManager usermanager,
                           ICacheManager<string, AppUserViewModel> userCache)
        {
            _userCache = userCache;
            _uow = uow;
            _userManager = usermanager;  
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

        public IEnumerable<ApplicationUser> GetUsers(List<string> userId)
        {
            var users = _uow.Context.Set<ApplicationUser>().AsNoTracking().Where(x => userId.Contains(x.Id)).AsEnumerable();

            return users;
        }

        public ApplicationUser GetUserInstance(string userName)
        {
            var user = _userManager.FindByName(userName);

            return user;
        }

        public AppUserViewModel GetCurrentUser(string userName)
        {
            AppUserViewModel appUser = _userCache.Get(userName);

            if (appUser == null)
            {
                //user = _userManager.FindByName(userName);
                ApplicationUser user = _uow.Context.Set<ApplicationUser>().AsNoTracking().FirstOrDefault(x => x.UserName == userName);

                if (user == null)
                {
                    return null;
                }

                appUser = new AppUserViewModel();

                appUser.FullName = user.FullName;
                appUser.Email = user.Email;
                appUser.IsManager = user.IsManager;
                appUser.Phone = user.Phone;
                appUser.Id = user.Id;

                _userCache.Set(userName, appUser);
            }

            return appUser;
        }

        public ApplicationUser Create(ApplicationUser item)
        {
            var result = _userManager.Create(item);

            if (result.Succeeded)
            {
                
            }
           
            return null;
        }

        public ApplicationUser Find(Guid id)
        {
            var user = _uow.Context.Set<ApplicationUser>().Find(id);

            return user;
        }

        public ApplicationUser Update(ApplicationUser item)
        {

            _uow.Context.Entry(item).State = EntityState.Modified;

            _uow.Context.SaveChanges();

            return item;
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
            var users = _uow.Context.Set<ApplicationUser>().AsNoTracking().OrderByDescending(x => x.Id).ToPagedList(pagenumber, pagesize);

            return users;
        }

        public IQueryable<ApplicationUser> GetTable()
        {
            return _uow.Context.Set<ApplicationUser>().AsNoTracking();
        }

        public ApplicationUser Find(string userId)
        {
            var user = _uow.Context.Set<ApplicationUser>().FirstOrDefault(x => x.Id == userId);

            return user;
        }
    }
}
