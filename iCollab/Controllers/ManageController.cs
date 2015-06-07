using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Core.Logging;
using Core.Service;
using Core.Settings;
using iCollab.Infra.Extensions;
using iCollab.ViewModels;
using Microsoft.AspNet.Identity;
using Model; 

namespace iCollab.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly ILogger _logger;

        public ManageController(IUserService userService, IApplicationSettings appSettings, ILogger logger) : base(userService, appSettings)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {  
            var model = new ProfileViewModel
            { 
                PhoneNumber = AppUser.Phone,
                FullName =  AppUser.FullName
 
            };
            return View(model);
        }

        [ChildActionOnly]
        public ActionResult ProfilePicture()
        {
            var profileViewModel = new ProfileViewModel {Attachment = AppUser.Picture};

            return PartialView("_ProfilePicture", profileViewModel);
        } 
         
        [HttpPost] 
        public ActionResult UploadPicture(HttpPostedFileBase file)
        {
            if (file != null)
            {

                string guidFilename = AttachmentHelper.CreateGuidFilename(file.FileName);
                string uploadPath = AttachmentHelper.GetUploadPath(guidFilename, AppSettings.ProfileServerPath);
                string accessPath = AttachmentHelper.GetAccessPath(guidFilename, AppSettings.ProfileAccessPath);

                try
                {

                    var workingDirectory = Path.GetDirectoryName(uploadPath);

                    if (workingDirectory == null)
                    {
                        throw new Exception("directory is null");
                    }

                    if (Directory.Exists(workingDirectory) == false)
                    {
                        Directory.CreateDirectory(workingDirectory);
                    }

                    file.SaveAs(uploadPath);

                    var attachment = new Attachment
                    {
                        Name = file.FileName,
                        Path = accessPath,
                        CreatedBy = User.Identity.GetUserName(),
                        DateCreated = DateTime.Now
                    };

                    var user = UserService.FindByUsername(AppUser.UserName);

                    user.Picture = attachment;

                    UserService.Update(user);

                    TempData["success"] = "Fotograf güncellendi.";
                    return RedirectToAction("Index", "Manage");
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                    TempData["error"] = "Hata oluştu.";
                    return RedirectToAction("Index", "Manage"); 
                }
                 
            }
            TempData["error"] = "Hata oluştu.";
            return RedirectToAction("Index", "Manage");
        }

        [ChildActionOnly]
        public ActionResult UpdateProfile()
        {
            var profileViewModel = new ProfileViewModel {FullName = AppUser.FullName, PhoneNumber = AppUser.Phone};

            return PartialView("_UpdateProfile", profileViewModel);
        }

        [HttpPost] 
        public ActionResult UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Hata oluştu.";
                return RedirectToAction("Index");
            }

            var user = UserService.FindByUsername(AppUser.UserName);

            user.Phone = model.PhoneNumber;
            user.FullName = model.FullName;

            var result = UserService.Update(user);

            if (result)
            {
                TempData["success"] = "Profil güncellendi.";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Hata oluştu.";
            return RedirectToAction("Index");
        }
         

        [ChildActionOnly]
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [HttpPost] 
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Hata oluştu.";
                return RedirectToAction("Index");
            }

            var result = UserService.ChangePassword(AppUser.Id, model.OldPassword, model.NewPassword);

            if (result)
            {
                TempData["success"] = "Şifre değiştirildi.";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Hata oluştu.";
            return RedirectToAction("Index");
        }

       /* 

       //
       // GET: /Manage/SetPassword
       public ActionResult SetPassword()
       {
           return View();
       }

       //
       // POST: /Manage/SetPassword
       [HttpPost]
       [ValidateAntiForgeryToken]
       public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
       {
           if (ModelState.IsValid)
           {
               var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
               if (result.Succeeded)
               {
                   var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                   if (user != null)
                   {
                       await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                   }
                   return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
               }
               AddErrors(result);
           }

           // If we got this far, something failed, redisplay form
           return View(model);
       }*/

 

#region Helpers 
         
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}