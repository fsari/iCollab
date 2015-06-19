using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Mvc.Mailer;

namespace Mailer
{
    public class MailMessage
    {
        private readonly MvcMailMessage _mail;
        private readonly bool _isDisabled;
        public MailMessage(MvcMailMessage mail, bool isDisabled)
        {
            _mail = mail;
            _isDisabled = isDisabled;
        }

        public void Send()
        {
            _mail.IsBodyHtml = true;
            _mail.Priority = MailPriority.High;
            if (_isDisabled == false)
            {
                _mail.Send();
            }
        }
    }
}
