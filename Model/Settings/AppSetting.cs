using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Settings
{
    public class AppSetting : BaseEntity, ISettings
    {

        public string AttachmentPath { set; get; }

    }
}
