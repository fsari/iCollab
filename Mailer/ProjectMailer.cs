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
        public MailMessage ProjectCreated(Project project, IEnumerable<string> users)
        { 
            var mail = new MvcMailMessage();
              
            users.Where(x=>string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "ProjectCreated";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;
            
            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        } 

        public MailMessage ProjectUpdated(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "ProjectUpdated";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectStatus(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "ProjectCreated";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectPriority(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "ProjectCreated";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage ProjectDeleted(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "ProjectCreated";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage AddedMeeting(Project project, Meeting meeting, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "AddedMeeting";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage AddedDocument(Project project, Document document, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "AddedDocument";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }

        public MailMessage AddedTask(Project project, Task task, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            users.Where(x => string.IsNullOrEmpty(x) == false).ToList().ForEach(mail.Bcc.Add);

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "AddedTask";

            var from = new MailAddress("dev@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            var mailMessage = new MailMessage(mail, _isDisabled);

            return mailMessage;
        }
    }
}
