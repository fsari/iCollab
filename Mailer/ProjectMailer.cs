using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public class ProjectMailer : MailerBase, IProjectMailer
    {
        public MvcMailMessage ProjectCreated(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user))
                {
                    continue;
                }

                mail.Bcc.Add(user);
            }

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "ProjectCreated";

            var from = new MailAddress("basin.haber@saglik.gov.tr", "Proje Geliştirme Uygulaması");
            mail.From = from;
            
            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            return mail;
        }

        public MvcMailMessage NotifyProjectUsers(Project project, IEnumerable<string> users)
        {
            var mail = new MvcMailMessage();

            foreach (var user in users)
            {
                if (string.IsNullOrEmpty(user))
                {
                    continue;
                }

                mail.Bcc.Add(user);
            }

            mail.IsBodyHtml = true;
            mail.Subject = project.Title;
            mail.Priority = MailPriority.High;
            mail.ViewName = "NotifyProjectUsers";

            var from = new MailAddress("basin.haber@saglik.gov.tr", "Basın haber uygulaması");
            mail.From = from;

            ViewBag.Project = project;

            PopulateBody(mail, mail.ViewName);

            return mail;
        }
    }
}
