using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public interface IProjectMailer
    {
        MvcMailMessage ProjectCreated(Project project, IEnumerable<string> users );
        MvcMailMessage NotifyProjectUsers(Project project, IEnumerable<string> users);



    }
}
