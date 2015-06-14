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
using Model;
using Model.Activity;
using Model.FineUploader;
using PagedList;

namespace iCollab.Controllers
{
    [Authorize]
    public class ProjectsController : BaseController
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper<DocumentViewModel, Document> _documentMapper;
        private readonly IMapper<ProjectViewModel, Project> _mapper;
        private readonly IMapper<MeetingViewModel, Meeting> _meetingMapper;
        private readonly IProjectService _projectService;
        private readonly IMapper<TaskViewModel, Task> _taskMapper;  

        public ProjectsController(
            IProjectService projectService,
            IApplicationSettings appSettings,
            IMapper<ProjectViewModel, Project> mapper,
            IMapper<TaskViewModel, Task> taskMapper,
            IUserService userService,
            IMapper<DocumentViewModel, Document> documentMapper,
            IMapper<MeetingViewModel, Meeting> meetingMapper,
            IAttachmentService attachmentService)
            : base(userService, appSettings)

        {
            _documentMapper = documentMapper;
            _meetingMapper = meetingMapper;
            _attachmentService = attachmentService; 
            _projectService = projectService;
            _mapper = mapper;
            _taskMapper = taskMapper; 
        }

        public ActionResult Index()
        {
            return View();
        }
         
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var user = UserService.FindById(AppUser.Id);
            var projects = _projectService.GetUserProjects(user);
            
            return Json(projects.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string query, int? page)
        {
            int pagenumber = page ?? 1;
            IPagedList<Project> resultset = _projectService.SearchUserProjects(query, AppUser.Id).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Project>
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
            IPagedList<Project> resultset = _projectService.SearchUserProjects(query, AppUser.Id).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Project>
            {
                Query = query,
                Page = pagenumber,
                Results = resultset,
                TotalItemCount = resultset.TotalItemCount
            };

            return View("Search", searchViewModel);
        }

        public ActionResult CompletedProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects = _projectService.GetProjectsByStatus(ProjectStatus.Bitti).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult LateProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects = _projectService.GetLateProjects().ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult ActiveProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects = _projectService.GetProjectsByStatus(ProjectStatus.Aktif).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult CancelledProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects = _projectService.GetProjectsByStatus(ProjectStatus.Iptal).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult PlannedProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects = _projectService.GetProjectsByStatus(ProjectStatus.Planlandı).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult AddMeeting(Guid? projectId)
        {
            if (projectId.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(projectId.Value);

            if (project == null)
            {
                return HttpNotFound();
            } 

            var documentViewModel = new MeetingViewModel {ProjectId = project.Id, DateTime = DateTime.Now.Date};

            return View(documentViewModel);
        }

        [HttpPost]
        public ActionResult AddMeeting(MeetingViewModel meetingViewModel)
        {
            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Bir hata oldu. Tekrar deneyiniz.";

                return View(meetingViewModel);
            }

            Project project = _projectService.GetProject(meetingViewModel.ProjectId, true);

            if (project == null)
            {
                return HttpNotFound();
            } 

            Meeting meeting = _meetingMapper.ToModel(meetingViewModel); 

            if (project.Meetings == null)
            {
                project.Meetings = new Collection<Meeting>();
            }

            var user = UserService.FindById(AppUser.Id);

            meeting.Owner = user;
            meeting.OwnerId = user.Id;

            project.Meetings.Add(meeting);

            _projectService.Update(project);

            TempData["success"] = "Toplantı oluşturuldu.";

            return RedirectToAction("View", new {id = project.Id});
        }

        public ActionResult AddDocument(Guid? projectId)
        {
            if (projectId.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(projectId.Value);

            if (project == null)
            {
                return HttpNotFound();
            } 

            var documentViewModel = new DocumentViewModel {ProjectId = project.Id};

            return View(documentViewModel);
        }

        [HttpPost]
        public ActionResult AddDocument(DocumentViewModel documentViewModel)
        {
            if (ModelState.IsValid == false)
            {
                return View(documentViewModel);
            }

            Project project = _projectService.GetProject(documentViewModel.ProjectId, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            Document document = _documentMapper.ToModel(documentViewModel); 

            if (project.Documents == null)
            {
                project.Documents = new Collection<Document>();
            }

            var user = UserService.FindById(AppUser.Id);

            document.Owner = user;
            document.OwnerId = user.Id;

            project.Documents.Add(document);

            _projectService.Update(project);

            TempData["success"] = "Doküman oluşturuldu.";

            return RedirectToAction("View", new {id = project.Id});
        }


        public ActionResult AddTask(Guid? projectId)
        {
            if (projectId.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(projectId.Value);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } 

            if (project.Status == ProjectStatus.Bitti || project.Status == ProjectStatus.Iptal)
            {
                ViewData["error"] = "Proje bitince veya iptal edilince görev ekleyemezsiniz.";
                return RedirectToAction("View", new {id = projectId});
            }

            var taskViewModel = new TaskViewModel {ProjectViewModel = _mapper.ToEntity(project), ProjectId = project.Id};

            return View(taskViewModel);
        }

        [HttpPost]
        public ActionResult AddTask(TaskViewModel taskViewModel)
        {

            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Bir hata olustu formu kontrol edip tekrar deneyiniz.";
                return View(taskViewModel);
            }

            Project project = _projectService.GetProject(taskViewModel.ProjectId, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
             

            if (taskViewModel.SelectedUsers == null || taskViewModel.SelectedUsers.Any() == false)
            {
                taskViewModel.ProjectViewModel = _mapper.ToEntity(project);
                ModelState.AddModelError("Error", "Bir kullanıcı seçmeniz gerekli.");

                return View(taskViewModel);
            }

            Task task = _taskMapper.ToModel(taskViewModel); 

            if (project.Tasks == null)
            {
                project.Tasks = new Collection<Task>();
            }

            task.ProjectId = project.Id;
            task.Project = project;

            var taskUser = UserService.FindById(AppUser.Id);

            task.TaskOwner = taskUser;
            task.TaskOwnerId = taskUser.Id;

            if (task.TaskUsers == null)
            {
               task.TaskUsers = new Collection<TaskUser>();
            }

            task.TaskUsers.AddRange(taskViewModel.SelectedUsers.Select(x => new TaskUser { UserId = x }));

            project.Tasks.Add(task);

            _projectService.Update(project);

            TempData["success"] = "Görev oluşturuldu.";

            return RedirectToAction("View", new {id = project.Id});
        }

        public ActionResult View(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(id.Value);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } 

            var projectUsers = project.ProjectUsers.Select(x=>x.UserId);
             
            var users = UserService.GetUsers(projectUsers.AsEnumerable());
             
            ProjectViewModel viewModel = _mapper.ToEntity(project); 

            var createdBy = UserService.FindByUsername(project.CreatedBy);

            viewModel.CreatedBy = createdBy.UserName;

            viewModel.SelectedUsers = users.Select(x => x.FullName).ToList();

            if (users.Any(x => x.Id != AppUser.Id))
            {
                return HttpNotFound();
            }

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var projectViewModel = new ProjectViewModel();

            return View(projectViewModel);
        }

        [HttpPost]
        public ActionResult Create(ProjectViewModel viewModel)
        {
            if (viewModel.SelectedUsers == null || viewModel.SelectedUsers.Any() == false)
            {
                ModelState.AddModelError("Error", "Kullanıcı seçmeniz lazım.");

                return View(viewModel);
            } 

            if (ModelState.IsValid)
            {
                Project project = _mapper.ToModel(viewModel);

                project.ProjectOwner = UserService.FindById(AppUser.Id);
                project.ProjectOwnerId = AppUser.Id;

                if (project.ProjectUsers == null)
                {
                    project.ProjectUsers = new Collection<ProjectUsers>();
                }
                 
                _projectService.Create(project);

                project.ProjectUsers.AddRange(viewModel.SelectedUsers.Select(x => new ProjectUsers { UserId = x }));

                _projectService.Update(project);

                if (project.Activities == null)
                {
                    project.Activities = new Collection<Activity>();
                }
  
                TempData["success"] = "Proje oluşturuldu.";
                return RedirectToAction("View", new {id = project.Id});
            }

            TempData["error"] = "Formu kontrol edip tekrar deneyiniz.";
            return View(viewModel);
        }

        public ActionResult Finish(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(id.Value, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.ProjectOwnerId != AppUser.Id)
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            project.Status = ProjectStatus.Bitti;
            project.DateCompleted = DateTime.Now;

            _projectService.Update(project);

            TempData["success"] = "Proje tamamlandı.";

            return RedirectToAction("View", new {project.Id});
        }

        public ActionResult CancelProject(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(id.Value, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.ProjectOwnerId != AppUser.Id)
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            project.Status = ProjectStatus.Iptal;
            project.DateCancelled = DateTime.Now;

            _projectService.Update(project);

            TempData["success"] = "Proje iptal edildi.";
            return RedirectToAction("View", new {project.Id});
        }

        public ActionResult Edit(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(id.Value);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (_projectService.ProjectUser(AppUser.Id, project.Id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProjectViewModel viewModel = _mapper.ToEntity(project);

            var userIds = project.ProjectUsers.Select(x => x.UserId);

            var users = UserService.GetUsers(userIds);

            viewModel.SelectedProjectUsers = users.Select(x => new UserSelectViewModel {FullName = x.FullName, Id = x.Id});

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ProjectViewModel viewModel)
        {

            if (viewModel.SelectedUsers == null || viewModel.SelectedUsers.Any() == false)
            { 
                TempData["error"] = "Kullanıcı seçmeniz lazım.";

                return View(viewModel);
            } 

            Project project = _projectService.GetProject(viewModel.Id, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (_projectService.ProjectUser(AppUser.Id, project.Id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                project.StartDatetime = viewModel.StartDatetime;
                project.EndDatetime = viewModel.EndDatetime;
                project.Priority = viewModel.Priority;
                project.Title = viewModel.Title;
                project.Description = viewModel.Description;

                project.ProjectUsers.Clear();

                _projectService.Update(project);

                project.ProjectUsers.AddRange(viewModel.SelectedUsers.Select(x => new ProjectUsers { UserId = x }));
  
                _projectService.Update(project);

                TempData["success"] = "Proje güncellendi.";

                return RedirectToAction("View", new {project.Id});
            }

            TempData["error"] = "Bir hata oluştu. Formu kontrol edip tekrar deneyiniz.";
            return View(viewModel);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Project project = _projectService.GetProject(id.Value, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.Status == ProjectStatus.Bitti)
            {
                TempData["success"] = "Bitmiş projeyi silemezsiniz.";
                return RedirectToAction("View", new {id});
            }

            if (_projectService.ProjectUser(AppUser.Id, project.Id) == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            project.DeletedBy = AppUser.UserName;
            project.DateDeleted = DateTime.UtcNow;

            _projectService.SoftDelete(project);

            TempData["success"] = "Proje silindi.";

            return RedirectToAction("Index");
        }

        public ActionResult RemoveAttachment(Guid projectId, Guid id)
        {
            Project task = _projectService.GetProject(projectId, true);

            Attachment attachment = _attachmentService.GetAttachment(id);

            if (attachment.CreatedBy == AppUser.UserName)
            {
                task.Attachments.Remove(attachment);

                _projectService.Update(task);

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
            string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, AppSettings.ProjectServerPath);
            string accessPath = AttachmentHelper.GetAccessPath(guidFilename, AppSettings.ProjectAccessPath);

            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment
                {
                    Name = upload.Filename,
                    Path = accessPath
                };

                Project project = _projectService.GetProject(id.Value, true);

                if (project == null)
                {
                    throw new Exception("Project is null");
                }

                if (project.Attachments == null)
                {
                    project.Attachments = new Collection<Attachment>();
                }

                project.Attachments.Add(attachment);

                _projectService.Update(project);

                string attachmentsHtml = RenderPartialViewToString("AttachmentList", new AttachmentViewModel
                {
                    Attachments = project.Attachments,
                    UploadPath = "/projects/UploadToProject/?id=" + project.Id,
                    RemovePath = "/projects/RemoveAttachment/?projectId=" + project.Id
                });

                return new FineUploaderResult(true, new { attachmentsHtml, accessPath });
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }

        public ActionResult GetProjectStatus()
        {
            var items = Enum.GetValues(typeof(ProjectStatus)).Cast<ProjectStatus>().Select(x=>x.DisplayName());

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeStatus(Guid pk, string name, string value)
        {
            var project = _projectService.GetProject(pk, true);

            var user = UserService.FindById(AppUser.Id);

            if (project.CanEditProject(user))
            {
                var newstatus = EnumExtensions.ParseEnum<ProjectStatus>(value);

                project.Status = newstatus;

                _projectService.Update(project);
                return Content("ok");
            }

            return Content("fail");
        }

        public ActionResult GetProjectPriority()
        {
            var items = Enum.GetValues(typeof(Priority)).Cast<Priority>().Select(x => x.DisplayName());

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePriority(Guid pk, string name, string value)
        {
            var project = _projectService.GetProject(pk, true);
             
            var user = UserService.FindById(AppUser.Id);

            if (project.CanEditProject(user))
            {
                var newstatus = EnumExtensions.ParseEnum<Priority>(value);

                project.Priority = newstatus;

                _projectService.Update(project); 

                return Content("ok");
            }

            return Content("fail");
        }

        
    }
}