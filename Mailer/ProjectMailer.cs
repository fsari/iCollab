using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail; 
using Model;
using Mvc.Mailer;
using Task = Model.Task;

namespace Mailer
{
    public interface IProjectMailer
    {
        MailMessage ProjectCreated(Project project, IEnumerable<string> users); 
        MailMessage ProjectUpdated(Project project, IEnumerable<string> users);
        MailMessage ProjectStatus(Project project, IEnumerable<string> users);
        MailMessage ProjectPriority(Project project, IEnumerable<string> users);
        MailMessage ProjectDeleted(Project project, IEnumerable<string> users);
        MailMessage AddedMeeting(Project project, Meeting meeting, IEnumerable<string> users);
        MailMessage AddedDocument(Project project, Document document, IEnumerable<string> users);
        MailMessage AddedTask(Project project, Task task, IEnumerable<string> users);
    }

    public class ProjectMailer : MailerBase, IProjectMailer
    {   
        private readonly bool _isDisabled = bool.Parse(ConfigurationManager.AppSettings["EmailDisabled"]);
        private const string From = "dev@saglik.gov.tr";
        private const string FromName = "Proje Geliştirme Uygulaması";

        public MailMessage ProjectCreated(Project project, IEnumerable<string> users)
        { 
            var mail = new MvcMailMessage();
              
            users.Where(x=>string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);
             
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

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }
    }
}
