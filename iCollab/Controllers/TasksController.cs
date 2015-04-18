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
    public class TasksController : BaseController
    {
        private readonly IMapper<TaskViewModel, Task> _mapper;
        private readonly IProjectService _projectService;
        private readonly ITaskService _service;
        private readonly IUserService _userService;
        private readonly IAttachmentService _attachmentService;

        public TasksController(
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
          
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = _service.GetTasks();
            return Json(tasks.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        } 
         
        public ActionResult RemoveAttachment(Guid taskId, Guid id)
        { 
            var task = _service.GetTask(taskId, true);

            var attachment = _attachmentService.GetAttachment(id);

            if (attachment.CreatedBy == AppUser.UserName)
            {
                task.Attachments.Remove(attachment);

                _service.Update(task);

                return Content("ok");
            }

            return Content("fail");
        }

        [HttpPost]
        public FineUploaderResult Upload(FineUpload upload, Guid? id)
        {
            if (id.HasValue == false)
            {
                return new FineUploaderResult(false, error: "id is null");
            }

            string guidFilename = AttachmentHelper.CreateGuidFilename(upload.Filename);

            string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, AppSettings.TaskServerPath);

            string accessPath = AttachmentHelper.GetAccessPath(guidFilename, AppSettings.TaskAccessPath);

            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment {Name = upload.Filename, Path = accessPath, CreatedBy = User.Identity.GetUserName()};

                Task task = _service.GetTask(id.Value,true);

                if (task == null)
                {
                    throw new Exception("task is null");
                }

                if (task.Attachments == null)
                {
                    task.Attachments = new Collection<Attachment>();
                }

                task.Attachments.Add(attachment);

                _service.Update(task);

                string attachmentsHtml = RenderPartialViewToString("AttachmentList", new AttachmentViewModel 
                                                                                            {
                                                                                                Attachments = task.Attachments, 
                                                                                                RemovePath = "/Tasks/RemoveAttachment/?taskId=" + task.Id
                                                                                            });
                 
                return new FineUploaderResult(true, new {extraInformation = 12345, attachmentsHtml, accessPath});
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }

        public ActionResult MyTasks(int? page)
        {
            int pagenumber = page ?? 1;

            string userId = User.Identity.GetUserId();

            IPagedList<Task> tasks = _service.GetUserTasks(userId).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        }

        public ActionResult CompletedTasks(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetTasksByStatus(TaskStatus.Tamamlandı).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        }

        public ActionResult RejectedTasks(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetTasksByStatus(TaskStatus.İade).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        }

        public ActionResult LateTasks(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetLateTasks().ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        }

        public ActionResult CompleteTask(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Task task = _service.GetTask(id.Value, nocache:true);

            if (task == null)
            {
                return HttpNotFound();
            }

            if (task.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string username = AppUser.UserName;

            /*if (task.TaskOwner != username)
            {
                return RedirectToAction("Unauthorized", "Error");
            }*/

            task.TaskStatus = TaskStatus.Tamamlandı;
            task.IsProcessed = true;
            task.DateCompleted = DateTime.Now;

            _service.Update(task);

            TempData["success"] = "Görev tamamlandı.";

            return RedirectToAction("View", new {id = id.Value});
        }

        public ActionResult ReturnTask(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Task task = _service.GetTask(id.Value);

            if (task == null)
            {
                return HttpNotFound();
            }

            if (task.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            string username = User.Identity.GetUserName();

            /*if (task.TaskOwner != username)
            {
                return RedirectToAction("Unauthorized", "Error");
            }*/

            ApplicationUser user = _userService.GetUserInstance(task.CreatedBy);

            task.TaskStatus = TaskStatus.İade;
            task.IsProcessed = true;
            task.DateCompleted = DateTime.Now;

            //TODO : alert task owner
            //task.TaskOwner = user.UserName;

            _service.Update(task);

            TempData["success"] = "Görev iade edildi.";
            return RedirectToAction("View", new {id = id.Value});
        }

        public ActionResult View(Guid? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task task = _service.GetTask(id.Value);

            if (task == null)
            {
                return new HttpNotFoundResult();
            }

            if (task.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TaskViewModel taskViewModel = _mapper.ToEntity(task);
            if (task.ProjectId != null) taskViewModel.ProjectId = task.ProjectId.Value;

            taskViewModel.SubTasks.RemoveAll(i=>i.IsDeleted);

            var taskUsers = task.TaskUsers.Select(x => x.UserId);

            var users = _userService.GetUsers(taskUsers.AsEnumerable());

            taskViewModel.SelectedUsers = users.Select(x => x.FullName).ToList();

            return View(taskViewModel);
        }

        public ActionResult Index()
        { 
            return View();
        }

        public ActionResult ViewCalendar()
        {

            return View();
        }

        public ActionResult Search(string query, int? page)
        {
            int pagenumber = page ?? 1;
            var resultset = _service.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Task>
            {
                Query = query,
                Page = pagenumber,
                Results = resultset,
                TotalItemCount = resultset.TotalItemCount
            };

            return View(searchViewModel);
        }

        public ActionResult NavigateSearch(string query, int? page)
        {
            int pagenumber = page ?? 1;
            var resultset = _service.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Task>
            {
                Query = query,
                Page = pagenumber,
                Results = resultset,
                TotalItemCount = resultset.TotalItemCount
            };

            return View("Search", searchViewModel);
        }

        public ActionResult AddSubTask(Guid parentId)
        { 
            var taskViewModel = new TaskViewModel
            { 
                ParentTaskId = parentId
            };

            var task = _service.GetTask(parentId);

            if (task.ProjectId.HasValue)
            {
                taskViewModel.ProjectId = task.ProjectId.Value;
            }

            return View(taskViewModel);
        }

        // TODO: Add parent task to errored requests
        [HttpPost]
        public ActionResult AddSubTask(TaskViewModel taskViewModel)
        {
            if (taskViewModel.SelectedUsers == null || taskViewModel.SelectedUsers.Any() == false)
            {
                ModelState.AddModelError("Error", "Kullanıcı seçmeniz lazım.");

                return View(taskViewModel);
            }


            if (taskViewModel.End < taskViewModel.Start)
            {
                taskViewModel.UserSelectList = _userService.GetUsersDropDown();

                ModelState.AddModelError("Error", "Bitiş tarihi başlangıç tarihinden erken olamaz.");

                return View(taskViewModel);
            } 

            if (taskViewModel.ParentTaskId.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task task = _mapper.ToModel(taskViewModel);

            task.CreatedBy = User.Identity.GetUserName(); 

            var parentTask = _service.GetTask(taskViewModel.ParentTaskId.Value, nocache: true);

            if (parentTask.SubTasks == null)
            {
                parentTask.SubTasks = new List<Task>();
            }

            task.ParentTaskId = taskViewModel.ParentTaskId;
            task.ParentTask = parentTask;

            parentTask.SubTasks.Add(task);

            _service.Update(parentTask);

            if (task.TaskUsers == null)
            {
                task.TaskUsers = new Collection<TaskUser>();
            }

            task.TaskUsers.AddRange(taskViewModel.SelectedUsers.Select(x => new TaskUser() { UserId = x }));

            _service.Update(task);

            TempData["success"] = "Görev oluşturuldu.";

            return RedirectToAction("View", new { id = task.Id });
        }

        public ActionResult CreateTask()
        {
            var taskViewModel = new TaskViewModel();

            return View(taskViewModel);
        }

        [HttpPost]
        public ActionResult CreateTask(TaskViewModel taskViewModel)
        {

            if (taskViewModel.SelectedUsers == null || taskViewModel.SelectedUsers.Any() == false)
            {
                ModelState.AddModelError("Error", "Kullanıcı seçmeniz lazım.");

                return View(taskViewModel);
            }

            if (taskViewModel.End < taskViewModel.Start)
            {
                taskViewModel.UserSelectList = _userService.GetUsersDropDown();

                ModelState.AddModelError("Error", "Bitiş tarihi başlangıç tarihinden erken olamaz.");

                return View(taskViewModel);
            } 

            Task task = _mapper.ToModel(taskViewModel);

            task.CreatedBy = AppUser.UserName;

            if (task.TaskUsers == null)
            {
                task.TaskUsers = new Collection<TaskUser>();
            } 

            _service.Create(task);

            task.TaskUsers.AddRange(taskViewModel.SelectedUsers.Select(x => new TaskUser() { UserId = x }));

            _service.Update(task);
            
            TempData["success"] = "Görev oluşturuldu.";

            return RedirectToAction("View", new {id = task.Id});
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task task = _service.GetTask(id.Value);

            if (task == null)
            {
                return HttpNotFound();
            }

            if (task.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (task.CreatedBy != AppUser.UserName)
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            TaskViewModel taskviewmodel = _mapper.ToEntity(task); 
             
            var userIds = task.TaskUsers.Select(x => x.UserId);

            var users = _userService.GetUsers(userIds);

            taskviewmodel.SelectedProjectUsers = users.Select(x => new UserSelectViewModel() { FullName = x.FullName, Id = x.Id });

            return View(taskviewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaskViewModel taskViewModel)
        {
            if (taskViewModel.SelectedUsers == null || taskViewModel.SelectedUsers.Any() == false)
            {
                TempData["error"] = "Kullanıcı seçmeniz lazım.";
                return View(taskViewModel);
            }

            if (taskViewModel.End < taskViewModel.Start)
            {
                taskViewModel.UserSelectList = _userService.GetUsersDropDown();

                TempData["error"] = "Bitiş tarihi başlangıç tarihinden erken olamaz.";

                return View(taskViewModel);
            } 

            if (ModelState.IsValid)
            {
                var instance = _service.GetTask(taskViewModel.Id, true);

                instance.EditedBy = AppUser.UserName;
                instance.DateEdited = DateTime.Now;
                instance.Title = taskViewModel.Title;
                instance.Description = taskViewModel.Description;
                instance.End = taskViewModel.End;
                instance.Start = taskViewModel.Start;

                _service.Update(instance);

                instance.TaskUsers.Clear();
                instance.TaskUsers.AddRange(taskViewModel.SelectedUsers.Select(x => new TaskUser() { UserId = x }));

                _service.Update(instance);

                TempData["success"] = "Görev güncellendi.";

                return RedirectToAction("View", new { taskViewModel.Id });
            }

            TempData["error"] = "Bir hata oluştu formu kontrol ediniz.";
            return View(taskViewModel);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task task = _service.GetTask(id.Value, nocache:true);

            if (task == null)
            {
                return HttpNotFound();
            }

            if (task.CreatedBy != AppUser.UserName)
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            if (task.SubTasks.Any(x=>x.IsDeleted == false))
            {
                TempData["error"] = "Alt görevi olan bir görevi silemezsiniz. Önce alt görevi silmeniz gerekli.";
                return RedirectToAction("View", new { id });
            }

            if (task.TaskStatus == TaskStatus.Tamamlandı)
            {
                TempData["error"] = "Tamamlanan bir görevi silemezsiniz";
                return RedirectToAction("View", new {id});
            }

            task.DeletedBy = AppUser.UserName;

            _service.SoftDelete(task);

            if (task.ParentTask != null)
            {
                _service.InvalidateCache(task.ParentTask.Id);
            }

            TempData["success"] = "Görev silinmiştir.";
            return RedirectToAction("Index");
        }
    }
}