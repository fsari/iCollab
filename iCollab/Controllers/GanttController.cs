using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web.Mvc; 
using Core.Mappers;
using Core.Service;
using Core.Settings;
using iCollab.Infra;
using iCollab.Infra.Extensions;
using iCollab.ViewModels;
using iCollab.ViewModels.Gantt;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Model;
using Model.FineUploader;
using PagedList;
using TaskViewModel = iCollab.ViewModels.TaskViewModel;

namespace iCollab.Controllers
{
    [Authorize]
    public class GanttController : BaseController
    {
        private readonly IMapper<TaskViewModel, Task> _mapper;
        private readonly IProjectService _projectService;
        private readonly ITaskService _service;
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;

        public GanttController(
            ITaskService service,
            IApplicationSettings appSettings,
            IMapper<TaskViewModel, Task> mapper,
            IUserService userService,
            IProjectService projectService,
            IAttachmentService attachmentService)
            : base(userService, appSettings)
        {
            _service = service;
            _mapper = mapper;
            _userService = userService;
            _projectService = projectService;
            _attachmentService = attachmentService;
        }

        public JsonResult Tasks([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = new List<GanttTaskViewModel>
            {
                new GanttTaskViewModel
                {
                    TaskID = 1,
                    Title = "My Project",
                    Start = new DateTime(2014, 8, 21, 11, 00, 00),
                    End = new DateTime(2014, 8, 25, 18, 30, 00),
                    Summary = true,
                    Expanded = true,
                    ParentID = null,
                    OrderId = 1
                },
                new GanttTaskViewModel
                {
                    TaskID = 2,
                    ParentID = 1,
                    Title = "Task 1",
                    Start = new DateTime(2014, 8, 21, 11, 00, 00),
                    End = new DateTime(2014, 8, 23, 14, 30, 00),
                    OrderId = 2
                },
                new GanttTaskViewModel
                {
                    TaskID = 3,
                    ParentID = 1,
                    Title = "Task 2",
                    Start = new DateTime(2014, 8, 21, 15, 00, 00),
                    End = new DateTime(2014, 8, 25, 18, 00, 00),
                    OrderId = 3
                }
            };

            return Json(tasks.AsQueryable().ToDataSourceResult(request));
        }

        public JsonResult Dependencies([DataSourceRequest] DataSourceRequest request)
        {
            var dependencies = new List<DependencyViewModel>
            {
                new DependencyViewModel {DependencyID = 100, PredecessorID = 2, SuccessorID = 3, Type = 0}
            };

            return Json(dependencies.AsQueryable().ToDataSourceResult(request));
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = _service.GetTasks();
            return Json(tasks.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }




    }
}