using System.Net.Mail;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public interface IAppMailer
    {
        MvcMailMessage Invite(ApplicationUser user); 
    }

    public sealed class AppMailer : MailerBase, IAppMailer
    {
        private const string Sender = "dev@saglik.gov.tr";

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

        public MvcMailMessage Invite(ApplicationUser user)
        {
            throw new System.NotImplementedException();
        }
    }
}