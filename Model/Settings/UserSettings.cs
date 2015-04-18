using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public class UserSettings : ISettings
    {

        public bool RecieveNotifications { set; get; }

        public bool Something { set; get; }

        public int SoemInt { set; get; }
    }
}
