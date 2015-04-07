using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using Mvc.Mailer;
using Task = System.Threading.Tasks.Task;

namespace Mailer
{
    public class TaskMailer : MailerBase, ITaskMailer
    {
        public MvcMailMessage TaskCreated(Task task, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public MvcMailMessage TaskEdited(Task task, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public MvcMailMessage TaskCompleted(Task task, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public MvcMailMessage TaskReturned(Task task, ApplicationUser user)
        {
            throw new NotImplementedException();
        }
    }
}
