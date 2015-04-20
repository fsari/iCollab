using System; 
using System.Linq; 
using System.Web.Mvc;
using Core.Mappers;
using Core.Service;
using Core.Settings;
using iCollab.Infra; 
using iCollab.ViewModels.Gantt;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI; 
using Model; 

namespace iCollab.Controllers
{
    [Authorize]
    public class GanttController : BaseController
    { 
        private readonly ITaskService _service;
        private readonly IMapper<Dependency, DependencyViewModel> _mapper;
        private readonly IDependencyService _dependencyService;

        public GanttController(
            ITaskService service,
            IApplicationSettings appSettings, 
            IUserService userService, 
            IMapper<Dependency, DependencyViewModel> mapper, 
            IDependencyService dependencyService)
            : base(userService, appSettings)
        {
            _service = service;
            _mapper = mapper;
            _dependencyService = dependencyService;
        }

        public JsonResult Tasks([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = _service.GetTasks().Select(x => new GanttTaskViewModel
            {
                TaskId = x.Id.ToString(), 
                Title = x.Title, 
                Start = x.Start, 
                End = x.End, 
                ParentId = x.ParentTaskId, 
                OrderId = x.OrderId, 
                PercentComplete = x.PercentComplete
            });

            return Json(tasks.AsQueryable().ToDataSourceResult(request));
        }
         
        public ActionResult Index()
        {
            return View();
        }

        public virtual JsonResult DestroyTask([DataSourceRequest] DataSourceRequest request, GanttTaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var instance = _service.GetTask(Guid.Parse(task.TaskId), true);

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
                    Title = task.Title,
                    Start = task.Start,
                    End = task.End,
                    ParentTaskId = task.ParentId,
                    PercentComplete = task.PercentComplete,
                    OrderId = task.OrderId
                }; 

                _service.Create(model);

                task.TaskId = model.Id.ToString();
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult UpdateTask([DataSourceRequest] DataSourceRequest request, GanttTaskViewModel task)
        {
            if (ModelState.IsValid)
            {
                var instance = _service.GetTask(Guid.Parse(task.TaskId), true);

                if (instance == null)
                {
                    return null;
                }

                instance.Title = task.Title;
                instance.Start = task.Start;
                instance.End = task.End;
                instance.ParentTaskId = task.ParentId;
                instance.PercentComplete = task.PercentComplete;
                instance.OrderId = task.OrderId;

                _service.Update(instance);
            }

            return Json(new[] { task }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult ReadDependencies([DataSourceRequest] DataSourceRequest request)
        {
            var dependencies = _dependencyService.GetTable()
                .Select(x => new DependencyViewModel() { Id = x.Id, PredecessorId = x.PredecessorId, SuccessorId = x.SuccessorId, Type = (Kendo.Mvc.UI.DependencyType)(int)x.Type }); 

            return Json(dependencies.ToDataSourceResult(request));
        }

        public virtual JsonResult DestroyDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                var entity = _mapper.ToEntity(dependency);

                _dependencyService.Delete(entity);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult CreateDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {
                var entity = _mapper.ToEntity(dependency);

                entity.Type = (Model.DependencyType)((int)dependency.Type);
                  
                _dependencyService.Create(entity);

                dependency.Id = entity.Id;
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }

        public virtual JsonResult UpdateDependency([DataSourceRequest] DataSourceRequest request, DependencyViewModel dependency)
        {
            if (ModelState.IsValid)
            {

                var entity = _mapper.ToEntity(dependency);

                _dependencyService.Update(entity);
            }

            return Json(new[] { dependency }.ToDataSourceResult(request, ModelState));
        }

    }
}