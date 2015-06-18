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

        public MailMessage TaskCreated(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "TaskCreated";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TaskEdited(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "TaskEdited";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TaskDeleted(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "TaskDeleted";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TaskCompleted(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "TaskCompleted";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage StatusChange(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "StatusChange";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage PriorityChange(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "PriorityChange";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage TypeChange(Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = task.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "TypeChange";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }
    }
}
