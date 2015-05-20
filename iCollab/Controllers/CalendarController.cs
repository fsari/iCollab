using System.Web.Mvc;
using Core.Settings;
using iCollab.Infra;

namespace iCollab.Controllers
{
    [Authorize]
    public class CalendarController : BaseController
    {
        public CalendarController(
            IApplicationSettings appSettings,
            IUserService userService)
            : base(userService, appSettings)
        {
        }

        public ActionResult ViewCalendar()
        {

            return View();
        }
         
    }
}