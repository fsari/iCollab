using System.Web.Mvc;

namespace iCollab.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    { 
        public ViewResult Index()
        {
            return View("Error");
        }
        public ViewResult NotFound()
        {
            Response.StatusCode = 404;  //you may want to set this to 200
            return View("NotFound");
        }

        public ViewResult InternalServerError()
        {
            return View("InternalServerError");
        }

        public ViewResult Unauthorized()
        {
            return View("Unauthorized");
        }
    }
}