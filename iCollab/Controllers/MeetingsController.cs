using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Core.Service;
using Core.Settings;
using iCollab.Infra;
using iCollab.Infra.Extensions;
using iCollab.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI; 
using Model;
using Model.FineUploader;
using PagedList;

namespace iCollab.Controllers
{
    [Authorize]
    public class MeetingsController : BaseController
    { 
        private readonly IMeetingService _service;
        private readonly IAttachmentService _attachmentService;
         
        public MeetingsController(
            IApplicationSettings settings,
            IMeetingService service,  
            IUserService userService, 
            IAttachmentService attachmentService
            ) : base(userService, settings)
        {
            _service = service;
            _attachmentService = attachmentService;
        }
         
        public ActionResult Index()
        {  
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<Meeting> meetings = _service.GetMeetings();
            return Json(meetings.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search(string query, int? page)
        {
            int pagenumber = page ?? 1;
            var resultset = _service.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Meeting>
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

            var searchViewModel = new SearchViewModel<Meeting>
            {
                Query = query,
                Page = pagenumber,
                Results = resultset,
                TotalItemCount = resultset.TotalItemCount
            };

            return View("Search", searchViewModel);
        }

        public ActionResult View(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Meeting meeting = _service.GetMeeting(id.Value);

            if (meeting == null)
            {
                return HttpNotFound();
            }

            if (meeting.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            } 

            return View(meeting);
        }

        public ActionResult Create()
        {
            var meeting = new Meeting {CreatedBy = AppUser.UserName, DateCreated = DateTime.Now};

            return View(meeting);
        }

        [HttpPost]
        public ActionResult Create(Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                meeting.CreatedBy = AppUser.UserName; 

                _service.Create(meeting);

                TempData["success"] = "Toplantı oluşturuldu.";
                return RedirectToAction("View", new{id= meeting.Id});
            }

            TempData["error"] = "Bir hata oldu.";
            return View(meeting);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Meeting meeting = _service.GetMeeting(id.Value);

            if (meeting.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(meeting);
        }

        [HttpPost]
        public ActionResult Edit(Meeting meeting)
        {
            if (meeting.IsDeleted)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                meeting.EditedBy = AppUser.UserName;

                _service.Update(meeting);
                 
                TempData["success"] = "Toplantı güncellendi.";

                return RedirectToAction("View", new { id = meeting.Id });
            }

            TempData["error"] = "Bir hata oldu.";
            return View(meeting);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Meeting meeting = _service.GetMeeting(id.Value);

            if (meeting == null)
            {
                return HttpNotFound();
            }

            meeting.DeletedBy = AppUser.UserName; 

            _service.SoftDelete(meeting);

            TempData["success"] = "Toplantı silindi.";
            return RedirectToAction("Index");
        }

        public ActionResult RemoveAttachment(Guid meetingId, Guid id)
        {
            var meeting = _service.GetMeeting(meetingId, true);

            var attachment = _attachmentService.GetAttachment(id);

            if (attachment.CreatedBy == AppUser.UserName)
            {
                meeting.Attachments.Remove(attachment);

                _service.Update(meeting);

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

            string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, AppSettings.MeetingServerPath);

            string accessPath = AttachmentHelper.GetAccessPath(guidFilename, AppSettings.MeetingAccessPath);

            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment { Name = upload.Filename, Path = accessPath, CreatedBy = AppUser.UserName};

                Meeting meeting = _service.GetMeeting(id.Value, true);

                if (meeting != null && meeting.Attachments == null)
                {
                    meeting.Attachments = new Collection<Attachment>();
                }

                if (meeting != null)
                {
                    meeting.Attachments.Add(attachment);

                    _service.Update(meeting);

                    string attachmentsHtml = RenderPartialViewToString("AttachmentList", new AttachmentViewModel
                    {
                        Attachments = meeting.Attachments,
                        RemovePath = "/Meetings/RemoveAttachment/?meetingId=" + meeting.Id
                    });

                    return new FineUploaderResult(true, new { extraInformation = 12345, attachmentsHtml, accessPath });
                }

                return null;
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }
    }
}