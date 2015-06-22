using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Core.Infrastructure;
using Core.Service;
using iCollab.Infra;
using iCollab.ViewModels;
using Mailer;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Model;

namespace iCollab.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IUserService _service;
        private IMailer _mailer;
          
        public AccountController(
            ApplicationUserManager userManager, 
            ApplicationSignInManager signInManager, 
            IUserService service, 
            IMailer mailer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _service = service;
            _mailer = mailer;
        }
          
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
         
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
             
            var result = _signInManager.PasswordSignIn(model.Email, model.Password, model.RememberMe, shouldLockout: false);

            if (result == SignInStatus.Success)
            {
                var user = _service.FindByEmail(model.Email);

                if (user.Disabled)
                {

                    Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction("Index", "Dashboard");
                }

                user.LastLogin = DateTime.Now;

                _service.Update(user);

                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        
        [Authorize]
        public ActionResult Register()
        {
            return View();
        }
         
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.Fullname,
                    DateCreated = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    TempData["success"] = "Kullanıcı kayıt edildi.";

                    return RedirectToAction("Index", "Users");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
         
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
         
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
         
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.FindByNameAsync(model.Email);
                // || !(await _userManager.IsEmailConfirmedAsync(user.Id))
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = Guid.NewGuid().ToString();

                user.PasswordResetToken = code;
                user.PasswordResetTokenUsed = false;
                var result = _userManager.Update(user);

                if (result.Succeeded)
                {
                    var callbackUrl = Url.Action("ResetPassword", "Account", new {userId = user.Id, code = code}, protocol: Request.Url.Scheme);
                     
                    _mailer.ForgotPassword(callbackUrl, model.Email).Send();

                    return RedirectToAction("ForgotPasswordConfirmation", "Account");
                }
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
             
            return View(model);
        }

         
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
         
        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string code)
        {
            var user = _userManager.FindById(userId);

            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (user.PasswordResetToken != code)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return code == null ? View("Error") : View();
        }
        
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            { 
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }

            if (user.PasswordResetTokenUsed ||
                (user.PasswordTokenUsedOn.HasValue && user.PasswordTokenUsedOn.Value.AddDays(2) < DateTime.Now))
            {
                return View("ForgotPasswordError");
            }

            var res = _userManager.RemovePassword(user.Id);

            if (res.Succeeded)
            {
                var createPassResult = _userManager.AddPassword(user.Id, model.Password);
                if (createPassResult.Succeeded)
                {
                    user.PasswordResetTokenUsed = true;
                    user.PasswordTokenUsedOn = DateTime.Now;
                    _userManager.Update(user);
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                AddErrors(createPassResult);
            }
            AddErrors(res);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
           
        [HttpPost] 
        public ActionResult LogOff()
        {
            Request.GetOwinContext().Authentication.SignOut();
            //AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Dashboard");
        } 
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Dashboard");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}