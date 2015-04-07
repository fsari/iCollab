using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iCollab.ViewModels
{
    public class CalendarEventItem
    {
        public string id { get; set; }
        public string title { get; set; }
        public bool IsAllDayEvent { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string url { set; get; }
        public string backgroundColor { set; get; }
        public string borderColor { set; get; }
    }
}