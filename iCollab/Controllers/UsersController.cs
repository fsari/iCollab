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
            IQueryable<UserSelectViewModel> users = _userService.GetAllUsers().Select(x => new UserSelectViewModel { Id = x.Id, FullName = x.FullName });
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "manager")]
        public ActionResult AssignAsManager(string userId)
        {
            ApplicationUser user = _userService.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            _userService.AssignManager(userId);

            TempData["success"] = "Kullanıcı yönetici olarak atandı.";

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "manager")]
        public ActionResult RemoveUser(string userId)
        {
            ApplicationUser user = _userService.Find(userId);

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