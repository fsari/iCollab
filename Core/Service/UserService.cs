using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Core.Caching;
using Core.Infrastructure; 
using Microsoft.AspNet.Identity;
using Model;
using PagedList;
using SharpRepository.Repository;

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
        IQueryable<ApplicationUser> GetUsers(IEnumerable<string> userId); 
        bool ChangePassword(string userId, string currentPassword, string newPassword);  
        bool Update(ApplicationUser user); 
        ApplicationUser FindByUsername(string userName);
        IEnumerable<string> GetUserEmailsByIds(IEnumerable<string> ids);
    }

    public class UserService : IUserService
    { 
        private readonly ApplicationUserManager _userManager; 
        private readonly ICacheManager _userCache;
        private readonly IRepository<ApplicationUser> _userRepository; 

        public UserService(ApplicationUserManager usermanager,
                           Func<string, ICacheManager> cache, 
                           IRepository<ApplicationUser> userRepository)
        {
            _userCache = cache("static"); 
            _userManager = usermanager;
            _userRepository = userRepository;
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
            var userCount = _userRepository.Count();

            return userCount;
        } 

        public bool IsUserManager(string userId)
        {
            return _userManager.IsInRole(userId, "manager");
        }
         
        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return _userRepository.AsQueryable();
        }
         

        public IQueryable<ApplicationUser> GetUsers(IEnumerable<string> userId)
        {
            var users = _userRepository.AsQueryable().AsNoTracking().Include(p=>p.Picture).Where(x => userId.Contains(x.Id));

            return users;
        }

        public ApplicationUser FindByUsername(string userName)
        { 
            var user =
                _userRepository.AsQueryable() 
                    .Include(p => p.Picture)
                    .FirstOrDefault(x => x.UserName == userName);

            return user;
        }

        public IEnumerable<string> GetUserEmailsByIds(IEnumerable<string> ids)
        {
            var emails = _userRepository.FindAll(x => ids.Contains(x.Id)).Select(x=>x.Email);

            return emails;
        }
        
        public ApplicationUser FindByEmail(string email)
        {
            var user = _userManager.FindByEmail(email);

            return user;
        } 

        public ApplicationUser FindById(string userId)
        {
            var user = _userRepository.AsQueryable().Include(p => p.Picture).FirstOrDefault(x => x.Id == userId); 

            return user;
        }

        public ApplicationUser GetCurrentUser(string userName)
        {
            ApplicationUser appUser = _userCache.Get(userName, () => FindByUsername(userName));
              
            return appUser;
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

        public ApplicationUser Delete(ApplicationUser user)
        {
            _userRepository.Delete(user);

            return user;
        } 

        public IPagedList<ApplicationUser> GetPageOf(int pagenumber, int pagesize)
        {
            var users = _userRepository.AsQueryable().AsNoTracking().Include(p=>p.Picture).AsNoTracking().OrderByDescending(x => x.Id).ToPagedList(pagenumber, pagesize);

            return users;
        }  
    }
}
