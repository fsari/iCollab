using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Core.Settings;
using iCollab.Infra;
using iCollab.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace iCollab.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        public ManageController(IUserService userService, IApplicationSettings appSettings) : base(userService, appSettings)
        {
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
        public ActionResult ChangePassword()
        {
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
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
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId"; 

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
         
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