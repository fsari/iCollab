﻿using System;
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
        private readonly ITaskService _service; 

        public GanttController(
            ITaskService service,
            IApplicationSettings appSettings, 
            IUserService userService)
            : base(userService, appSettings)
        {
            _service = service; 
        }

        public JsonResult Tasks([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = _service.GetTasks().Select(x => new GanttTaskViewModel
            {
                TaskID = x.Id, 
                Title = x.Title, 
                Start = x.Start, 
                End = x.End, 
                ParentID = x.ParentTaskId, 
                OrderId = x.OrderId, 
                PercentComplete = x.PercentComplete
            });

            return Json(tasks.AsQueryable().ToDataSourceResult(request));
        }

        public JsonResult Dependencies([DataSourceRequest] DataSourceRequest request)
        {
            var dependencies = new List<DependencyViewModel>();

            return Json(dependencies.AsQueryable().ToDataSourceResult(request));
        }

        public ActionResult Index()
        {
            return View();
        }

        public virtual JsonResult DestroyTask([DataSourceRequest] DataSourceRequest request, GanttTaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var instance = _service.GetTask(task.TaskID, true);

                _service.Delete(instance);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateTask([DataSourceRequest] DataSourceRequest request, GanttTaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var model = new Task
                {
                    Id = task.TaskID,
                    Title = task.Title,
                    Start = task.Start,
                    End = task.End,
                    ParentTaskId = task.ParentID,
                    PercentComplete = task.PercentComplete,
                    OrderId = task.OrderId
                };

                _service.Create(model);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult UpdateTask([DataSourceRequest] DataSourceRequest request, GanttTaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var instance = _service.GetTask(task.TaskID, true);
                instance.Title = task.Title;
                instance.Start = task.Start;
                instance.End = task.End;
                instance.ParentTaskId = task.ParentID;
                instance.PercentComplete = task.PercentComplete;
                instance.OrderId = task.OrderId;

                _service.Update(instance);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

    }
}