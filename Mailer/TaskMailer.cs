using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Model;
using Mvc.Mailer;
using Task = Model.Task;

namespace Mailer
{
    public interface ITaskMailer
    {
        MailMessage TaskCreated(Task task, IEnumerable<string> users);
        MailMessage TaskEdited(Task task, IEnumerable<string> users);
        MailMessage TaskDeleted(Task task, IEnumerable<string> users);
        MailMessage TaskCompleted(Task task, IEnumerable<string> users);
        MailMessage StatusChange(Task task, IEnumerable<string> users);
        MailMessage PriorityChange(Task task, IEnumerable<string> users);
        MailMessage TypeChange(Task task, IEnumerable<string> users); 

    }

    public class TaskMailer : MailerBase, ITaskMailer
    {
        private readonly bool _isDisabled = bool.Parse(ConfigurationManager.AppSettings["EmailDisabled"]);
        private const string From = "dev@saglik.gov.tr";
        private const string FromName = "Proje Geliştirme Uygulaması";

        public MailMessage TaskCreated(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "TaskCreated";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TaskEdited(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "TaskEdited";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TaskDeleted(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "TaskDeleted";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TaskCompleted(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "TaskCompleted";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage StatusChange(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "StatusChange";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage PriorityChange(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "PriorityChange";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TypeChange(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
            mail.Subject = task.Title; 
            mail.ViewName = "TypeChange";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }
    }
}
