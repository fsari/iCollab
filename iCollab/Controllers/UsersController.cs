using System;
using System.Linq;
using System.Web.Mvc;
using Core.Service;
using Core.Settings;
using iCollab.Infra;
using iCollab.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Model;
using PagedList;

namespace iCollab.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;

        public UsersController(IUserService userService, IApplicationSettings appSettings, ITaskService taskService) : base(userService, appSettings)
        {
            _userService = userService;
            _taskService = taskService;
        }

        public ActionResult Index(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<ApplicationUser> users = _userService.GetPageOf(pagenumber, AppSettings.PageSize);

            return View(users);
        }

        public ActionResult GetUsers()
        {
            var users = _userService.GetAllUsers().Select(x => new UserSelectViewModel { Id = x.Id, FullName = x.FullName });
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [ChildActionOnly]
        public ActionResult RenderUserImage(string username)
        {
            var user = _userService.GetCurrentUser(username);

            return PartialView("_RenderUserImage",user);
        }

        public ActionResult GetProjectUsers(Guid projectId)
        {
            var userIds = _userService.GetProjectUsers(projectId);

            var users = _userService.GetUsers(userIds.Select(x => x.UserId)); 

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "manager")]
        public ActionResult AssignAsManager(string userId)
        {
            ApplicationUser user = _userService.FindById(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            _userService.AssignManager(userId);

            TempData["success"] = "Kullanıcı yönetici olarak atandı.";

            return RedirectToAction("Index");
        }

        public ActionResult DisableUser(string userId)
        {
            ApplicationUser user = _userService.FindById(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            user.Disabled = true;

            _userService.Update(user);

            TempData["success"] = "Kullanıcı disable edildi.";

            return RedirectToAction("Index");
        }

        public ActionResult EnableUser(string userId)
        {
            ApplicationUser user = _userService.FindById(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            user.Disabled = false;

            _userService.Update(user);

            TempData["success"] = "Kullanıcı disable edildi.";

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "manager")]
        public ActionResult RemoveUser(string userId)
        {
            ApplicationUser user = _userService.FindById(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            bool hasAnyTasks = _taskService.UserHasAnyTasks(user.Id);

            if (hasAnyTasks)
            {
                return View("RemoveTasks");
            }

            _userService.Delete(user);

            TempData["success"] = "Kullanıcı silindi.";

            return RedirectToAction("Index");
        }
    }

    
}