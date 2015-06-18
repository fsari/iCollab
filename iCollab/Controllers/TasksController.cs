using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Core.Extensions;
using Core.Mappers;
using Core.Service;
using Core.Settings; 
using iCollab.Infra.Extensions;
using iCollab.ViewModels; 
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mailer;
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
        private readonly ITaskService _service; 
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper<AppUserViewModel, ApplicationUser> _userMapper;
        private readonly IProjectService _projectService;
        private readonly ITaskMailer _mailer;

        public TasksController(
            ITaskService service,
            IApplicationSettings appSettings,
            IMapper<TaskViewModel, Task> mapper,
            IUserService userService,
            IProjectService projectService, 
            IAttachmentService attachmentService, 
            IMapper<AppUserViewModel, ApplicationUser> userMapper, 
            ITaskMailer mailer)
            : base(userService, appSettings)
        {
            _service = service;
            _mapper = mapper; 
            _attachmentService = attachmentService;
            _userMapper = userMapper;
            _mailer = mailer;
            _projectService = projectService;
            _mailer = mailer;
        }
          
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var tasks = _service.GetUserTasks(AppUser.Id).Select(x=> new TaskViewModel{Id = x.Id, Title = x.Title, Start = x.Start, End = x.End, Priority = x.Priority , TaskStatus = x.TaskStatus});
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

                var attachment = new Attachment {Name = upload.Filename, Path = accessPath};

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

        public ActionResult CompletedTasks(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetTasksByStatus(TaskStatus.Tamamlandı).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        } 

        public ActionResult LateTasks(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetLateTasks().ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        }

        public ActionResult My(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetUserTasks(AppUser.Id).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(tasks);
        }

        public ActionResult ByMe(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Task> tasks = _service.GetTasksUserCreated(AppUser.Id).ToPagedList(pagenumber, AppSettings.PageSize);

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
              
            task.TaskStatus = TaskStatus.Tamamlandı; 
            task.DateCompleted = DateTime.Now;

            _service.Update(task);

            if (task.ProjectId.HasValue)
            {
                _projectService.InvalidateCache(task.ProjectId.Value);
            }

            TempData["success"] = "Görev tamamlandı.";

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

            if (task.TaskUsers.Any(x => x.UserId == AppUser.Id) == false && task.TaskOwnerId != AppUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (task.Project != null)
            {
                if (task.Project.IsDeleted)
                {
                    return HttpNotFound();
                }
            }

            TaskViewModel taskViewModel = _mapper.ToEntity(task);
            if (task.ProjectId != null) taskViewModel.ProjectId = task.ProjectId.Value;

            taskViewModel.SubTasks.RemoveAll(i=>i.IsDeleted);

            var taskUsers = task.TaskUsers.Select(x => x.UserId);

            var users = UserService.GetUsers(taskUsers.AsEnumerable());

            taskViewModel.SelectedUsers = users.Select(x => x.UserName).ToList(); 

            taskViewModel.TaskOwner = _userMapper.ToEntity(task.TaskOwner);

            return View(taskViewModel);
        }

        // TODO : email task owner that user began task
        public ActionResult Begin(Guid? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task task = _service.GetTask(id.Value,true);

            if (task == null)
            {
                return new HttpNotFoundResult();
            }

            if (task.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (task.TaskUsers.Any(x => x.UserId == AppUser.Id) == false && task.TaskOwnerId != AppUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            task.TaskStatus = TaskStatus.Aktif;
            _service.Update(task);

            return RedirectToAction("View", new {id = id.Value});
        }

        public ActionResult Index()
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
            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Bir hata olustu formu kontrol edip tekrar deneyiniz.";
                return View(taskViewModel);
            }

            if (taskViewModel.SelectedUsers == null || taskViewModel.SelectedUsers.Any() == false)
            {
                TempData["error"] = "Bir hata olustu formu kontrol edip tekrar deneyiniz.";

                return View(taskViewModel);
            }

            if (taskViewModel.ParentTaskId.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Task task = _mapper.ToModel(taskViewModel);

            if (task.ParentTaskId.HasValue == false)
            {
                throw new Exception("Parent task Id can not be null.");
            }

            var parentTask = _service.GetTask(task.ParentTaskId.Value, nocache: true);

            if (parentTask.SubTasks == null)
            {
                parentTask.SubTasks = new List<Task>();
            }

            task.ParentTaskId = task.ParentTaskId.Value;
            task.ParentTask = parentTask;

            var taskUser = UserService.FindById(AppUser.Id);

            task.TaskOwner = taskUser;
            task.TaskOwnerId = taskUser.Id;

            parentTask.SubTasks.Add(task);

            _service.Update(parentTask);

            if (task.TaskUsers == null)
            {
                task.TaskUsers = new Collection<TaskUser>();
            }

            task.TaskUsers.AddRange(taskViewModel.SelectedUsers.Select(x => new TaskUser { UserId = x }));

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

            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Bir hata olustu formu kontrol edip tekrar deneyiniz.";
                return View(taskViewModel);
            }

            if (taskViewModel.SelectedUsers == null || taskViewModel.SelectedUsers.Any() == false)
            {
                TempData["error"] = "Bir hata olustu formu kontrol edip tekrar deneyiniz.";

                return View(taskViewModel);
            }
             
            Task task = _mapper.ToModel(taskViewModel);
            task.TaskOwnerId = AppUser.Id;

            var taskOwner = UserService.FindById(task.TaskOwnerId);
            task.TaskOwner = taskOwner;

            task.TaskOwnerId = taskOwner.Id; 

            _service.Create(task);

            if (task.TaskUsers == null)
            {
                task.TaskUsers = new Collection<TaskUser>();
            }

            task.TaskUsers.AddRange(task.SelectedUsers.Select(x => new TaskUser { UserId = x }));

            _service.Update(task);

            var userEmails = UserService.GetUserEmailsByIds(task.SelectedUsers.Select(i => i));
            _mailer.TaskCreated(task, userEmails).Send();
            
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

            var users = UserService.GetUsers(userIds);

            taskviewmodel.SelectedProjectUsers = users.Select(x => new UserSelectViewModel { FullName = x.FullName, Id = x.Id });

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
              
            if (ModelState.IsValid)
            {
                var instance = _service.GetTask(taskViewModel.Id, true);
                 
                instance.Title = taskViewModel.Title;
                instance.Description = taskViewModel.Description;
                instance.End = taskViewModel.End;
                instance.Start = taskViewModel.Start;
                instance.Priority = taskViewModel.Priority;


                _service.Update(instance);

                instance.TaskUsers.Clear();
                instance.TaskUsers.AddRange(taskViewModel.SelectedUsers.Select(x => new TaskUser { UserId = x }));

                _service.Update(instance);

                var userEmails = UserService.GetUserEmailsByIds(instance.SelectedUsers.Select(i => i));
                _mailer.TaskEdited(instance, userEmails).Send();

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
            task.DateEdited = DateTime.UtcNow;

            _service.SoftDelete(task);

            if (task.ParentTask != null)
            {
                _service.InvalidateCache(task.ParentTask.Id);
            }

            var userEmails = UserService.GetUserEmailsByIds(task.SelectedUsers.Select(i => i));
            _mailer.TaskDeleted(task, userEmails).Send();

            TempData["success"] = "Görev silinmiştir.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ChangeProgress(string progress, Guid id)
        {
            var task = _service.GetTask(id, true);

            if (task == null)
            {
                return Content("fail");
            }

            int prog;

            if (int.TryParse(progress, out prog))
            {
                if (prog%10 == 0 && prog < 101)
                {
                    task.Progress = prog;
                    _service.Update(task);
                    return Content("ok");
                }
            }

            return Content("fail");
        }

        public ActionResult GetTaskStatus()
        {
            var items = Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>().Select(x => x.DisplayName());

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatus(Guid pk, string name, string value)
        {
            var task = _service.GetTask(pk, true);

            var user = UserService.FindById(AppUser.Id);

            if (task.CanEditProject(user))
            {
                var newstatus = EnumExtensions.ParseEnum<TaskStatus>(value);

                task.TaskStatus = newstatus;

                _service.Update(task);

                var userEmails = UserService.GetUserEmailsByIds(task.SelectedUsers.Select(i => i));
                _mailer.StatusChange(task, userEmails).Send();

                return Content("ok");
            }

            return Content("fail");
        }

        public ActionResult GetTaskTypes()
        {
            var items = Enum.GetValues(typeof(TaskType)).Cast<TaskType>().Select(x => x.DisplayName());

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeTaskType(Guid pk, string name, string value)
        {
            var task = _service.GetTask(pk, true);

            var user = UserService.FindById(AppUser.Id);

            if (task.CanEditProject(user))
            {
                var newType = EnumExtensions.ParseEnum<TaskType>(value);

                task.TaskType = newType;

                _service.Update(task);

                var userEmails = UserService.GetUserEmailsByIds(task.SelectedUsers.Select(i => i));
                _mailer.StatusChange(task, userEmails).Send();

                return Content("ok");
            }

            return Content("fail");
        }

        public ActionResult GetTaskPriority()
        {
            var items = Enum.GetValues(typeof(Priority)).Cast<Priority>().Select(x => x.DisplayName());

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePriority(Guid pk, string name, string value)
        {
            var task = _service.GetTask(pk, true);

            var user = UserService.FindById(AppUser.Id);

            if (task.CanEditProject(user))
            {
                var newpriority = EnumExtensions.ParseEnum<Priority>(value);

                task.Priority = newpriority;

                _service.Update(task);

                var userEmails = UserService.GetUserEmailsByIds(task.SelectedUsers.Select(i => i));
                _mailer.StatusChange(task, userEmails).Send();

                return Content("ok");
            }

            return Content("fail");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SaveDescription(Guid id, string data)
        {
            var task = _service.GetTask(id, true);

            var user = UserService.FindById(AppUser.Id);

            if (task.CanEditProject(user))
            { 
                task.Description = data;

                _service.Update(task);
                return Content("ok");
            }

            return Content("fail");
        }
    }
}