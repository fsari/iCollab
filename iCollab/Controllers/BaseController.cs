using System.Web.Mvc;
using Core.Mappers;
using Core.Service;
using Core.Settings; 
using iCollab.Infra.Extensions;
using iCollab.ViewModels;
using Microsoft.AspNet.Identity;
using Model;

namespace iCollab.Controllers
{
    public class BaseController : MController
    {

        protected readonly IUserService UserService;
        protected AppUserViewModel AppUser;
        protected IApplicationSettings AppSettings; 

        public BaseController(IUserService userService, IApplicationSettings appSettings)
        {
            UserService = userService;
            AppSettings = appSettings; 
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext.Response.Headers.Add("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");

            var user = UserService.GetCurrentUser(User.Identity.GetUserName());

            IMapper<ApplicationUser, AppUserViewModel> mapper = new Mapper<ApplicationUser, AppUserViewModel>();

            AppUser = mapper.ToModel(user);

            if (UserService.IsUserManager(AppUser.Id))
            {
                AppUser.IsManager = true;
            }
              
            ViewBag.IsManager = AppUser.IsManager;

            base.OnActionExecuting(filterContext);
        }
    }
}