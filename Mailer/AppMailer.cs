using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public interface IMailer
    {
        MvcMailMessage Invite(ApplicationUser user);

        MailMessage ProjectCreated(Project project, IEnumerable<string> users);
        MailMessage ProjectUpdated(Project project, IEnumerable<string> users);
        MailMessage ProjectStatus(Project project, IEnumerable<string> users);
        MailMessage ProjectPriority(Project project, IEnumerable<string> users);
        MailMessage ProjectDeleted(Project project, IEnumerable<string> users);
        MailMessage AddedMeeting(Project project, Meeting meeting, IEnumerable<string> users);
        MailMessage AddedDocument(Project project, Document document, IEnumerable<string> users);
        MailMessage AddedTask(Project project, Task task, IEnumerable<string> users);

        MailMessage TaskCreated(Task task, IEnumerable<string> users);
        MailMessage TaskEdited(Task task, IEnumerable<string> users);
        MailMessage TaskDeleted(Task task, IEnumerable<string> users);
        MailMessage TaskCompleted(Task task, IEnumerable<string> users);
        MailMessage StatusChange(Task task, IEnumerable<string> users);
        MailMessage PriorityChange(Task task, IEnumerable<string> users);
        MailMessage TypeChange(Task task, IEnumerable<string> users);
        MailMessage TaskBegan(Task task, IEnumerable<string> users);
    }

    public sealed class Mailer : MailerBase, IMailer
    {
        private readonly bool _isDisabled = bool.Parse(ConfigurationManager.AppSettings["EmailDisabled"]);
        private const string From = "dev@saglik.gov.tr";
        private const string FromName = "Proje Geliştirme Uygulaması";

        public MvcMailMessage Welcome(ApplicationUser user)
        { 
/*             
            ViewBag.Name = user.UserName;
            return Populate(x =>
            {
                x.ViewName = "Welcome";
                x.To.Add(user.UserName);
                x.Subject = "Hoşgeldiniz";
                x.From = new MailAddress(Sender);
                x.IsBodyHtml = true;
            });*/
            return null;
        }


        public MvcMailMessage Invite(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }

        public MailMessage ProjectCreated(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "ProjectCreated";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);  

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectUpdated(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "ProjectUpdated";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectStatus(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "ProjectStatus";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectPriority(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "ProjectPriority";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectDeleted(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "ProjectDeleted";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage AddedMeeting(Project project, Meeting meeting, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "AddedMeeting";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;
            ViewBag.Meeting = meeting;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage AddedDocument(Project project, Document document, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "AddedDocument";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;
            ViewBag.Document = document;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage AddedTask(Project project, Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.Subject = project.Title;
            mail.ViewName = "AddedTask";

            mail.From = new MailAddress(From, FromName);

            ViewBag.Project = project;
            ViewBag.Task = task;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

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

        public MailMessage TaskBegan(Task task, IEnumerable<string> users)
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