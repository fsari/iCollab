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
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;
using Model;
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
        private readonly IUserService _userService;

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
            _userService = userService;
        }

        public ActionResult Index()
        {
            return View();
        }
         
        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var projects = _projectService.GetProjects();
            return Json(projects.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string query, int? page)
        {
            int pagenumber = page ?? 1;
            IPagedList<Project> resultset = _projectService.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Project>();
            searchViewModel.Query = query;
            searchViewModel.Page = pagenumber;
            searchViewModel.Results = resultset;
            searchViewModel.TotalItemCount = resultset.TotalItemCount;

            return View(searchViewModel);
        }

        public ActionResult NavigateSearch(string query, int? page)
        {
            int pagenumber = page ?? 1;
            IPagedList<Project> resultset = _projectService.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Project>();
            searchViewModel.Query = query;
            searchViewModel.Page = pagenumber;
            searchViewModel.Results = resultset;
            searchViewModel.TotalItemCount = resultset.TotalItemCount;

            return View("Search", searchViewModel);
        }

        public ActionResult CompletedProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects =
                _projectService.GetProjectsByStatus(ProjectStatus.Bitti).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult LateProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects = _projectService.GetLateProjects()
                .ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult ActiveProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects =
                _projectService.GetProjectsByStatus(ProjectStatus.Aktif).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult CancelledProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects =
                _projectService.GetProjectsByStatus(ProjectStatus.Iptal).ToPagedList(pagenumber, AppSettings.PageSize);

            return View(projects);
        }

        public ActionResult PlannedProjects(int? page)
        {
            int pagenumber = page ?? 1;

            IPagedList<Project> projects =
                _projectService.GetProjectsByStatus(ProjectStatus.Planlandı)
                    .ToPagedList(pagenumber, AppSettings.PageSize);

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

            meeting.CreatedBy = User.Identity.GetUserName();

            if (project.Meetings == null)
            {
                project.Meetings = new Collection<Meeting>();
            }

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

            document.CreatedBy = User.Identity.GetUserName();

            if (project.Documents == null)
            {
                project.Documents = new Collection<Document>();
            }

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
            Project project = _projectService.GetProject(taskViewModel.ProjectId, true);

            if (project == null)
            {
                return HttpNotFound();
            }

            if (project.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid == false || string.IsNullOrEmpty(taskViewModel.SelectedUserId))
            {
                taskViewModel.ProjectViewModel = _mapper.ToEntity(project);
                ModelState.AddModelError("Error", "Bir kullanıcı seçmeniz gerekli.");

                return View(taskViewModel);
            }

            Task task = _taskMapper.ToModel(taskViewModel);

            task.CreatedBy = User.Identity.GetUserName();

            ApplicationUser assignee = _userService.Find(taskViewModel.SelectedUserId);

            task.TaskOwner = assignee.UserName;

            if (project.Tasks == null)
            {
                project.Tasks = new Collection<Task>();
            }

            task.ProjectId = project.Id;
            task.Project = project;

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

            ProjectViewModel viewModel = _mapper.ToEntity(project); 

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

            if (viewModel.EndDatetime.HasValue)
            {
                if (viewModel.StartDatetime.HasValue == false)
                {
                    viewModel.StartDatetime = DateTime.Now;
                }

                if (viewModel.EndDatetime.Value < viewModel.StartDatetime.Value)
                {
                    ModelState.AddModelError("Error", "Bitiş tarihi başlangıç tarihinden erken olamaz.");

                    return View(viewModel);
                }
            }

            if (ModelState.IsValid)
            {
                Project project = _mapper.ToModel(viewModel);
                 
                project.ProjectOwner = AppUser.UserName;
                project.CreatedBy = AppUser.UserName;

                if (project.ProjectUsers == null)
                {
                    project.ProjectUsers = new Collection<ProjectUsers>();
                }
                 
                _projectService.Create(project);

                project.ProjectUsers.AddRange(viewModel.SelectedUsers.Select(x => new ProjectUsers() { UserId = x }));

                _projectService.Update(project);

                TempData["success"] = "Proje oluşturuldu.";

                return RedirectToAction("View", new {id = project.Id});
            }

            ModelState.AddModelError("Error", "Formu kontrol edip tekrar deneyiniz.");
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

            if (project.ProjectOwner != User.Identity.GetUserId())
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

            if (project.ProjectOwner != User.Identity.GetUserId())
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

            if (project.CreatedBy != User.Identity.GetUserName())
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            ProjectViewModel viewModel = _mapper.ToEntity(project);

            var userIds = project.ProjectUsers.Select(x => x.UserId);

            var users = _userService.GetUsers(userIds);

            viewModel.SelectedUsers = users.Select(x => x.FullName).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ProjectViewModel viewModel)
        {
            if (viewModel.EndDatetime.HasValue)
            {
                if (viewModel.StartDatetime.HasValue == false)
                {
                    viewModel.StartDatetime = DateTime.Now;
                }

                if (viewModel.EndDatetime.Value < viewModel.StartDatetime.Value)
                {
                    ModelState.AddModelError("Error", "Bitiş tarihi başlangıç tarihinden erken olamaz.");

                    return View(viewModel);
                }
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

            if (ModelState.IsValid)
            {
                project.StartDatetime = viewModel.StartDatetime;
                project.EndDatetime = viewModel.EndDatetime;
                project.Priority = viewModel.Priority;
                project.Title = viewModel.Title;
                project.Description = viewModel.Description;


                _projectService.Update(project);

                TempData["success"] = "Proje güncellendi.";

                return RedirectToAction("View", new {project.Id});
            }

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
                return RedirectToAction("View", new {id});
            }

            if (project.ProjectOwner != User.Identity.GetUserName())
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            project.DeletedBy = User.Identity.GetUserName();

            _projectService.SoftDelete(project);

            TempData["success"] = "Proje silindi.";

            return RedirectToAction("Index");
        }

        public ActionResult RemoveAttachment(Guid projectId, Guid id)
        {
            Project task = _projectService.GetProject(projectId, true);

            Attachment attachment = _attachmentService.GetAttachment(id);

            if (attachment.CreatedBy == User.Identity.GetUserName())
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
                    Path = accessPath,
                    CreatedBy = User.Identity.GetUserName()
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
                }
                    );

                return new FineUploaderResult(true, new {extraInformation = 12345, attachmentsHtml, accessPath});
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }
    }
}