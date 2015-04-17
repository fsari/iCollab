using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Core.Mappers;
using Core.Service;
using Core.Service.CrudService;
using Core.Settings;
using iCollab.Infra;
using iCollab.ViewModels;
using Microsoft.AspNet.Identity;
using Model;
using Model.Activity;
using PagedList;

namespace iCollab.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        private readonly IProjectService _projectService;
        private readonly IMapper<ProjectViewModel, Project> _mapper;
        private readonly ITaskService _taskService;
        private readonly IAnnouncementService _announcementsService; 
        private readonly IUserService _userService;
        private readonly IMeetingService _meetingService;
        private readonly IDocumentService _documentService;
        private readonly ICrudService<Activity> _activityService;

        public DashboardController(
            IProjectService projectService,
            IApplicationSettings appSettings,
            IMapper<ProjectViewModel, Project> mapper,
            IAnnouncementService announcementsService,
            IUserService userService,
            ITaskService taskService, 
            IMeetingService meetingService,
            IDocumentService documentService,
            ICrudService<Activity> activityService)
            : base(userService, appSettings)
        {
            _projectService = projectService;
            _mapper = mapper;
            _announcementsService = announcementsService;
            _userService = userService;
            _taskService = taskService; 
            _meetingService = meetingService;
            _documentService = documentService;
            _activityService = activityService;
        }

        [ChildActionOnly]
        public ActionResult ViewTasks()
        {
            var userId = User.Identity.GetUserId();
            var tasks = _taskService.GetUserTasks(userId).Take(AppSettings.IndexPageSize);

            return PartialView(tasks);
        }

        [ChildActionOnly]
        public ActionResult ViewTodos()
        {
            return PartialView();
        }

        public dynamic GetData(int year)
        {

            var projectCreated = (from project in _projectService.GetTable()
                                  where project.DateCreated.Year == year
                                  group project by project.DateCreated.Month
                                      into grp
                                      select new
                                      {
                                          Month = grp.Key,
                                          Projects = grp.Count()
                                      }).ToList();

            var taksCreated = (from item in _taskService.GetTable()
                               where item.DateCreated.Year == year
                               group item by item.DateCreated.Month
                                   into grp
                                   select new
                                   {
                                       Month = grp.Key,
                                       Tasks = grp.Count()
                                   }).ToList();



            var tasksCompleted = (from task in _taskService.GetTable()
                                  where task.TaskStatus == TaskStatus.Tamamlandı && (!task.DateCompleted.HasValue || task.DateCompleted.Value.Year == year)
                                  group task by task.DateCompleted.Value.Month
                                      into grp
                                      select new
                                      {
                                          Month = grp.Key,
                                          Tasks = grp.Count()
                                      }).ToList();



            var ret = new[]
            { 
                new { label="Görevler", data = taksCreated.Select(x=>new[]{ x.Month, x.Tasks })},
                new { label="Tamamlanan Görevler", data = tasksCompleted.Select(x=>new[]{ x.Month, x.Tasks })},
                new { label="Projeler", data = projectCreated.Select(x=>new[]{ x.Month, x.Projects })}
            };

            var json = new JavaScriptSerializer().Serialize(ret);

            return json;
        }

        public JsonResult GetCalendarEvents(string start, string end)
        {
            var fromDate = DateTime.Parse(start);

            var toDate = DateTime.Parse(end);

            string user = User.Identity.GetUserName();

            /*var tasks = _taskService.GetTasks().
                Where(x => x.StartDatetime > fromDate && x.EndDatetime < toDate && x.TaskOwner == user && x.TaskStatus != TaskStatus.İade)
                .Select(y => new CalendarEventItem() { end = y.EndDatetime.Value, start = y.StartDatetime.Value, title = y.Title, id = y.Id.ToString(), url = "/Tasks/View/" + y.Id }).ToArray();*/
/*

            var items = tasks.Select(y => new { end = DateTimeToUnixTimestamp(y.end), start = DateTimeToUnixTimestamp(y.start), title = y.title, id = y.id, url = "/Tasks/View/" + y.id });
*/


            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (dateTime - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {

            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return origin.AddSeconds(timestamp);

        }

        [ChildActionOnly]
        public ActionResult GetOnlineUsers()
        {
            var onlineusers = _userService.GetOnlineUsers();

            return PartialView(onlineusers);
        }

        [OutputCache(Duration = 300)]
        [ChildActionOnly]
        public ActionResult QuickReports()
        {
            var userCount = _userService.GetUserCount();
            var projectCount = _projectService.ProjectsCount();
            var announcementCount = _announcementsService.AnnouncementsCount();
            var taskCount = _taskService.TasksCount(); 
            var meetingCount = _meetingService.MeetingsCount();

            var quickReportViewModel = new QuickReportsViewModel
            {
                AnnouncementsCount = announcementCount,
                TasskCount = taskCount,
                ProjectsCount = projectCount,
                UsersCount = userCount, 
                MeetingCount = meetingCount
            };

            return PartialView(quickReportViewModel);
        }

        [ChildActionOnly]
        public ActionResult ViewProjects()
        {
            string user = User.Identity.GetUserName();

            var projects = _projectService.GetUserProjects(user).ToPagedList(1, AppSettings.IndexPageSize);

            var pageOfprojectViewModels = _mapper.ToEntities(projects);

            return PartialView(pageOfprojectViewModels);
        }

        [ChildActionOnly]
        public ActionResult ViewActivities()
        {
            var activities = _activityService.GetTable().AsNoTracking().OrderByDescending(x => x.Id).Take(AppSettings.IndexPageSize);

            return PartialView(activities);
        }

        [ChildActionOnly]
        public ActionResult ViewAnnouncements()
        {
            var announcements = _announcementsService.GetPublishedAnnouncements().ToPagedList(1, AppSettings.IndexPageSize);

            return PartialView(announcements);
        }

        [ChildActionOnly]
        public ActionResult ViewDocuments()
        {
            string username = User.Identity.GetUserName();

            var documents = _documentService.UserDocuments(username).ToPagedList(1, AppSettings.IndexPageSize);

            return PartialView(documents);
        }


        [ChildActionOnly]
        public ActionResult ViewMeetings()
        {
            string username = User.Identity.GetUserName();

            var meetings = _meetingService.GetUserMeetings(username).ToPagedList(1, AppSettings.IndexPageSize);

            return PartialView(meetings);
        }

        [ChildActionOnly]
        public ActionResult Sidebar()
        {
            var rd = ControllerContext.ParentActionViewContext.RouteData;
            var action = rd.GetRequiredString("action");
            var controller = rd.GetRequiredString("controller");

            ViewBag.Active = controller;

            return PartialView(AppUser);
        }

        [ChildActionOnly]
        public ActionResult RenderHeader()
        {
            return PartialView(AppUser);
        }

        public ActionResult Index()
        {
            return View();
        }
    }
}