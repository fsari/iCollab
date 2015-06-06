using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Core.Logging;
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
        private readonly IUserService _userService;
        private readonly IMeetingService _meetingService;
        private readonly IDocumentService _documentService;
        private readonly ICrudService<Activity> _activityService;
        private readonly ILogger _logger;

        public DashboardController(
            IProjectService projectService,
            IApplicationSettings appSettings,
            IMapper<ProjectViewModel, Project> mapper, 
            IUserService userService,
            ITaskService taskService, 
            IMeetingService meetingService,
            IDocumentService documentService,
            ICrudService<Activity> activityService, ILogger logger)
            : base(userService, appSettings)
        {
            _projectService = projectService;
            _mapper = mapper; 
            _userService = userService;
            _taskService = taskService; 
            _meetingService = meetingService;
            _documentService = documentService;
            _activityService = activityService;
            _logger = logger;
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
          

        [OutputCache(Duration = 300)]
        [ChildActionOnly]
        public ActionResult QuickReports()
        {
            var userCount = _userService.GetUserCount();
            var projectCount = _projectService.ProjectsCount(); 
            var taskCount = _taskService.TasksCount(); 
            var meetingCount = _meetingService.MeetingsCount();

            var quickReportViewModel = new QuickReportsViewModel
            { 
                TasskCount = taskCount,
                ProjectsCount = projectCount,
                UsersCount = userCount, 
                MeetingCount = meetingCount
            };

            return PartialView(quickReportViewModel);
        }

        [ChildActionOnly]
        public ActionResult ViewTasks()
        { 
            var tasks = _taskService.GetUserTasks(AppUser.Id).Take(AppSettings.IndexPageSize);

            return PartialView(tasks);
        }

        [ChildActionOnly]
        public ActionResult ViewProjects()
        { 

            var projects = _projectService.GetUserProjects(AppUser.Id).ToPagedList(1, AppSettings.IndexPageSize);

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
        public ActionResult ViewDocuments()
        {

            var documents = _documentService.UserDocuments(AppUser.UserName).ToPagedList(1, AppSettings.IndexPageSize);

            return PartialView(documents);
        }


        [ChildActionOnly]
        public ActionResult ViewMeetings()
        {

            var meetings = _meetingService.GetUserMeetings(AppUser.UserName).ToPagedList(1, AppSettings.IndexPageSize);

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
            _logger.Info("test");
            return View();
        }
    }
}