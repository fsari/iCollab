using Model;
using Mvc.Mailer;

namespace Mailer
{
    public interface IAppMailer
    {
        MvcMailMessage Welcome(ApplicationUser user);  

    }
}