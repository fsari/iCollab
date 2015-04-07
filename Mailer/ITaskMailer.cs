using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using Model;
using Mvc.Mailer;
using Task = System.Threading.Tasks.Task;

namespace Mailer
{
    public interface ITaskMailer
    {
        MvcMailMessage TaskCreated(Task task, ApplicationUser user);
        MvcMailMessage TaskEdited(Task task, ApplicationUser user);
        MvcMailMessage TaskCompleted(Task task, ApplicationUser user);
        MvcMailMessage TaskReturned(Task task, ApplicationUser user);

    }
}
