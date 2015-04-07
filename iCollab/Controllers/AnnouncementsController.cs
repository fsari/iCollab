using System;
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
    public class AnnouncementsController : BaseController
    {
        private readonly IAnnouncementService _announcementService;
        private readonly IMapper<AnnouncementViewModel, Announcement> _mapper;
        private readonly IAttachmentService _attachmentService;

        public AnnouncementsController(
            IAnnouncementService announcementService,
            IMapper<AnnouncementViewModel, Announcement> mapper,
            IApplicationSettings appSettings,
            IUserService userService, 
            IAttachmentService attachmentService)
            : base(userService, appSettings)
        {
            _announcementService = announcementService;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public ActionResult Index(int? page)
        { 
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<Announcement> announcements = _announcementService.GetAnnouncements();

            return Json(announcements.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Search(string query, int? page)
        {
            int pagenumber = page ?? 1;
            var resultset = _announcementService.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Announcement>
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
            var resultset = _announcementService.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Announcement>
            {
                Query = query,
                Page = pagenumber,
                Results = resultset,
                TotalItemCount = resultset.TotalItemCount
            };

            return View("Search", searchViewModel);
        }

        public ActionResult Create()
        {
            var announcement = new AnnouncementViewModel { CreatedBy = User.Identity.GetUserName()};

            return View(announcement);
        }

        [HttpPost]
        public ActionResult Create(AnnouncementViewModel announcement)
        {
            if (announcement.PublishDate > announcement.EndDate)
            {
                TempData["error"] = "Bitiş tarihi başlangıç tarihinden önce olamaz.";
                return View(announcement);
            }

            if (ModelState.IsValid)
            {
                Announcement model = _mapper.ToModel(announcement);

                model.CreatedBy = User.Identity.GetUserName();

                Announcement instance = _announcementService.Create(model);

                TempData["success"] = "Duyuru oluşturulmuştur.";
                return RedirectToAction("View", new {id = instance.Id});
            }

            return View(announcement);
        }

        public ActionResult View(Guid? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Announcement announcement = _announcementService.GetAnnouncement(id.Value);

            if (announcement == null)
            {
                return HttpNotFound();
            }

            if (announcement.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AnnouncementViewModel viewModel = _mapper.ToEntity(announcement);

            return View(viewModel);
        }
         
        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Announcement announcement = _announcementService.GetAnnouncement(id.Value, true);

            if (announcement == null)
            {
                return HttpNotFound();
            }

            if (announcement.CreatedBy != User.Identity.GetUserName())
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            announcement.DeletedBy = User.Identity.GetUserName();
            announcement.IsDeleted = true;
            announcement.DateDeleted = DateTime.Now;

            _announcementService.Update(announcement);

            TempData["success"] = "Duyuru silimiştir.";
            return RedirectToAction("Index");
        }

        public ActionResult Edit(Guid? id)
        {
            if (id.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Announcement announcement = _announcementService.GetAnnouncement(id.Value, true);

            if (announcement == null)
            {
                return HttpNotFound();
            }

            if (announcement.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (announcement.CreatedBy != User.Identity.GetUserName())
            {
                return RedirectToAction("Unauthorized", "Error");
            }

            AnnouncementViewModel viewModel = _mapper.ToEntity(announcement);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(AnnouncementViewModel announcement)
        {
            if (announcement.PublishDate > announcement.EndDate)
            {
                TempData["error"] = "Bitiş tarihi başlangıç tarihinden önce olamaz.";
                return View(announcement);
            }

            if (ModelState.IsValid)
            {
                Announcement model = _mapper.ToModel(announcement);

                if (model.IsDeleted)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                model.EditedBy = User.Identity.GetUserName();

                Announcement instance = _announcementService.Update(model);

                TempData["success"] = "Duyuru düzenlenmiştir.";
                return RedirectToAction("View", new {id = instance.Id});
            }

            return View(announcement);
        }

        public ActionResult RemoveAttachment(Guid announcementId, Guid id)
        {
            var announcement = _announcementService.GetAnnouncement(announcementId, true);

            var attachment = _attachmentService.GetAttachment(id);

            if (attachment.CreatedBy == User.Identity.GetUserName())
            {
                announcement.Attachments.Remove(attachment);

                _announcementService.Update(announcement);

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

            string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, AppSettings.AnnouncementServerPath);

            string accessPath = AttachmentHelper.GetAccessPath(guidFilename, AppSettings.AnnouncementAccessPath);
            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment {Name = upload.Filename, Path = accessPath, CreatedBy = User.Identity.GetUserName()};

                Announcement announcement = _announcementService.GetAnnouncement(id.Value, true);

                if (announcement == null)
                {
                    throw new Exception("Announcement is null");
                }

                if (announcement.IsDeleted)
                {
                    return null;
                }

                if (announcement.Attachments == null)
                {
                    announcement.Attachments = new Collection<Attachment>();
                }

                announcement.Attachments.Add(attachment);

                _announcementService.Update(announcement);

                string attachmentsHtml = RenderPartialViewToString("AttachmentList", new AttachmentViewModel 
                                                                                            {
                                                                                                Attachments = announcement.Attachments,
                                                                                                RemovePath = "/Announcements/RemoveAttachment/?announcementId=" + announcement.Id
                                                                                            });

                // the anonymous object in the result below will be convert to json and set back to the browser
                return new FineUploaderResult(true, new {extraInformation = 12345, attachmentsHtml, accessPath});
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }
    }
}