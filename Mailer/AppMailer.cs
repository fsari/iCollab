using System.Net.Mail;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public sealed class AppMailer : MailerBase, IAppMailer
    {
        private const string Sender = "ashtakip@saglik.gov.tr";

        public MvcMailMessage Welcome(ApplicationUser user)
        {
            ViewBag.Name = user.UserName;
            return Populate(x =>
            {
                x.ViewName = "Welcome";
                x.To.Add(user.UserName);
                x.Subject = "Hoşgeldiniz";
                x.From = new MailAddress(Sender);
                x.IsBodyHtml = true;
            });
        } 

        public MvcMailMessage TaskAssigned(ApplicationUser user, Task task)
        {
            ViewBag.Name = user.UserName;
            ViewBag.Title = task.Title;
            ViewBag.TaskId = task.Id;
            return Populate(x =>
            {
                x.ViewName = "TaskAssigned";
                x.To.Add(user.UserName);
                x.Subject = "Görev atandı";
                x.From = new MailAddress(Sender);
                x.IsBodyHtml = true;
            });
        }

        public MvcMailMessage TaskReturned(ApplicationUser user, Task task)
        {
            ViewBag.Name = user.UserName;
            ViewBag.Title = task.Title;
            ViewBag.TaskId = task.Id;
            return Populate(x =>
            {
                x.ViewName = "Görev iade edildi";
                x.To.Add(user.UserName);
                x.Subject = "Görev iade edildi.";
                x.From = new MailAddress(Sender);
                x.IsBodyHtml = true;
            });
        }

        public MvcMailMessage TaskCompleted(ApplicationUser user, Task task)
        {
            ViewBag.Name = user.UserName;
            ViewBag.Title = task.Title;
            ViewBag.TaskId = task.Id;
            return Populate(x =>
            {
                x.ViewName = "Görev tamamlandı";
                x.To.Add(user.UserName);
                x.Subject = "Görev tamamlandı.";
                x.From = new MailAddress(Sender);
                x.IsBodyHtml = true;
            });
        }

        public MvcMailMessage ProjectCompleted(ApplicationUser user, Project project)
        {
            ViewBag.Name = user.UserName;
            ViewBag.Title = project.Title;
            ViewBag.ProjectId = project.Id;
            return Populate(x =>
            {
                x.ViewName = "Proje tamamlandı";
                x.To.Add(user.UserName);
                x.Subject = "Proje tamamlandı.";
                x.From = new MailAddress(Sender);
                x.IsBodyHtml = true;
            });
        }
    }
}