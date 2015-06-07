using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Core.Mappers;
using Core.Service;
using Core.Settings; 
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
    public class DocumentsController : BaseController
    {
        private readonly IDocumentService _service;
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper<ContentPage, DocumentContentPageViewModel> _contentPageMapper; 


        public DocumentsController(
            IApplicationSettings appSettings,
            IDocumentService service, 
            IAttachmentService attachmentService,
            IMapper<ContentPage, DocumentContentPageViewModel> contentPageMapper, 
            IUserService userService) 
            : base(userService, appSettings)
        {
            _service = service;
            _attachmentService = attachmentService;
            _contentPageMapper = contentPageMapper; 
        }

        public ActionResult Index(int? page)
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var documents  = _service.UserDocuments(AppUser.UserName);
            return Json(documents.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        } 

        [HttpPost]
        public ActionResult Search(string query, int? page)
        {
            int pagenumber = page ?? 1;
            var resultset = _service.Search(query).ToPagedList(pagenumber, AppSettings.PageSize);

            var searchViewModel = new SearchViewModel<Document>
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

            var searchViewModel = new SearchViewModel<Document>
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

            Document document = _service.GetDocument(id.Value);

            if (document == null)
            {
                return HttpNotFound();
            }

            if (document.IsPublic == false && document.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }

            return View(document);
        }

        public ActionResult AddContentPage(Guid documentId)
        {
            var document = _service.GetDocument(documentId);

            if (document.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }
 
            var contentPage = new DocumentContentPageViewModel {DocumentGuid = documentId};

            return View(contentPage);
        }

        [HttpPost]
        public ActionResult AddContentPage(DocumentContentPageViewModel documentContentPageViewModel)
        {
            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Bir sorun oldu.";
                return View(documentContentPageViewModel);
            }

            Document document = _service.GetDocument(documentContentPageViewModel.DocumentGuid, true);
             
            if (document == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (document.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }

            if (document.ContentPages == null)
            {
                document.ContentPages = new Collection<ContentPage>();
            }

            var user = UserService.FindById(AppUser.Id);

            var contentPage = new ContentPage
            {
                Title = documentContentPageViewModel.Title,
                Description = documentContentPageViewModel.Description,
                CreatedBy = AppUser.UserName,
                DateCreated = DateTime.Now,
                OwnerId = AppUser.Id,
                Owner = user
            };

            document.ContentPages.Add(contentPage);

            _service.Update(document);

            TempData["success"] = "İçerik eklendi.";

            return RedirectToAction("View", new {id = document.Id});
        }

        public ActionResult DeleteContentPage(Guid? documentId, Guid? contentPageId)
        {
            if (documentId.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (contentPageId.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var document = _service.GetDocument(documentId.Value, nocache: true);

            if (document == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contentPage = document.ContentPages.FirstOrDefault(x => x.Id == contentPageId.Value);

            if (contentPage == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (contentPage.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }

            contentPage.DeletedBy = AppUser.UserName;
            contentPage.DateDeleted = DateTime.UtcNow;

            document.ContentPages.Remove(contentPage);

            _service.Update(document);

            TempData["success"] = "İçerik silindi.";

            return RedirectToAction("View", new { id = documentId.Value });
        }

        public ActionResult EditContentPage(Guid? documentId, Guid? contentPageId)
        {
            if (documentId.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (contentPageId.HasValue == false)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var document = _service.GetDocument(documentId.Value);

            if (document == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contentPage = document.ContentPages.FirstOrDefault(x => x.Id == contentPageId.Value);

            if (contentPage == null)
            {
                return HttpNotFound();
            }

            if (contentPage.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }

            var viewModel = _contentPageMapper.ToModel(contentPage);
            viewModel.DocumentGuid = documentId.Value;

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult EditContentPage(DocumentContentPageViewModel viewModel)
        {
            if (ModelState.IsValid == false)
            {
                TempData["error"] = "Bir sorun oluştu.";
                return View(viewModel);
            }

            var document = _service.GetDocument(viewModel.DocumentGuid, nocache:true);

            if (document == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var contentPage = document.ContentPages.FirstOrDefault(x => x.Id == viewModel.Id);

            if (contentPage == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (contentPage.OwnerId != AppUser.Id)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            contentPage.Title = viewModel.Title; 
            contentPage.Description = viewModel.Description;

            _service.Update(document);
            TempData["success"] = "İçerik düzenledi.";
            return RedirectToAction("View", new {id = viewModel.DocumentGuid});
        }
         
        public ActionResult Create()
        {
            var document = new Document ();

            return View(document);
        }

        [HttpPost]
        public ActionResult Create(Document document)
        {
            if (ModelState.IsValid)
            {

                var user = UserService.FindById(AppUser.Id);

                document.Owner = user;
                document.OwnerId = AppUser.Id;

                Document createdItem = _service.Create(document);
                  
                TempData["success"] = "Döküman oluşturuldu.";
                return RedirectToAction("View", new {id = createdItem.Id});
            }

            TempData["error"] = "Bir sorun oluştu.";
            return View(document);
        }


        public ActionResult Edit(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Document document = _service.GetDocument(id.Value, true);

            if (document == null)
            {
                return HttpNotFound();
            }

            if (document.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }

            return View(document);
        }

        [HttpPost]
        public ActionResult Edit(Document document)
        {
            if (ModelState.IsValid)
            {
                if (document.IsDeleted)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (document.OwnerId != AppUser.Id)
                {
                    return new HttpUnauthorizedResult();
                }

                _service.Update(document);

                TempData["success"] = "Döküman güncellendi.";
                return RedirectToAction("View", new {id = document.Id});
            }

            TempData["error"] = "Bir sorun oluştu.";
            return View();
        }

        public ActionResult Delete(Guid? id)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Document document = _service.GetDocument(id.Value,true);

            if (document == null)
            {
                return HttpNotFound();
            }

            if (document.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            } 

            _service.SoftDelete(document);

            TempData["success"] = "Döküman silindi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult MakePublic(Guid? id, bool isPublic)
        {
            if (id.HasValue == false)
            {
                return HttpNotFound();
            }

            Document document = _service.GetDocument(id.Value,true);

            if (document == null)
            {
                return HttpNotFound();
            }

            if (document.OwnerId != AppUser.Id)
            {
                return new HttpUnauthorizedResult();
            }

            document.IsPublic = isPublic;
            
            _service.Update(document);

            return Content("ok");
        }

        public ActionResult RemoveAttachment(Guid documentId, Guid id)
        {
            var document = _service.GetDocument(documentId, true);

            var attachment = _attachmentService.GetAttachment(id);

            if (attachment.CreatedBy == AppUser.UserName)
            {
                document.Attachments.Remove(attachment);

                _service.Update(document);

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

            string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, AppSettings.DocumentsServerPath);

            string accessPath = AttachmentHelper.GetAccessPath(guidFilename, AppSettings.DocumentsAccessPath);
            try
            {
                upload.SaveAs(uploadPath);

                var attachment = new Attachment {Name = upload.Filename, Path = accessPath};

                Document document = _service.GetDocument(id.Value, true);

                if (document == null)
                {
                    throw new Exception("Announcement is null");
                }

                if (document.Attachments == null)
                {
                    document.Attachments = new Collection<Attachment>();
                }

                document.Attachments.Add(attachment);

                _service.Update(document);

                string attachmentsHtml = RenderPartialViewToString("AttachmentList", new AttachmentViewModel 
                                                                                            {
                                                                                                Attachments = document.Attachments, 
                                                                                                RemovePath = "/Documents/RemoveAttachment/?documentId=" + document.Id
                                                                                            });
                 
                return new FineUploaderResult(true, new {extraInformation = 12345, attachmentsHtml, accessPath});
            }
            catch (Exception ex)
            {
                return new FineUploaderResult(false, error: ex.Message);
            }
        }
    }
}