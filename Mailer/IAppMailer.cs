using Model;
using Mvc.Mailer;

namespace Mailer
{
    public interface IAppMailer
    {
        MvcMailMessage Welcome(ApplicationUser user); 
        MvcMailMessage TaskAssigned(ApplicationUser user, Task task);
        MvcMailMessage TaskReturned(ApplicationUser user, Task task);
        MvcMailMessage TaskCompleted(ApplicationUser user, Task task);
        MvcMailMessage ProjectCompleted(ApplicationUser user, Project project);

    }
}