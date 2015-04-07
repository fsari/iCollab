using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Mvc.Mailer;

namespace Mailer
{
    public class AnnouncementMailer  :MailerBase, IAnnouncementMailer
    {
        public MvcMailMessage Announce(Announcement announcement, IEnumerable<string> users)
        {
            throw new NotImplementedException();
        }
    }
}
