using System.Collections.Generic;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public interface IProjectMailer
    {
        MvcMailMessage ProjectCreated(Project project, IEnumerable<string> users );
        MvcMailMessage NotifyProjectUsers(Project project, IEnumerable<string> users);
        MvcMailMessage ProjectUpdated(Project project, IEnumerable<string> users);
    }
}
