using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCollab.Infra
{
    public class StoreDependingSettingData
    {
        public StoreDependingSettingData()
        {
            OverrideSettingKeys = new List<string>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }
        public List<string> OverrideSettingKeys { get; set; }
        public string RootSettingClass { get; set; }
    }
}