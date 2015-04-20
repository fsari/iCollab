using System;
using System.Globalization;

namespace Model.Activity
{
    public class Activity : BaseEntity
    { 

        public string Username{ get; set; }
        public ActivityType ActivityType { get; set; }
        public string Title { set; get; }
        public string Url { set; get; }

        public Subject Subject { set; get; }

        public override string ToString()
        {
            string action = string.Empty;
            string subject = string.Empty;

            if (ActivityType == ActivityType.Created)
            {
                action = "oluşturdu";
            }

            if (ActivityType == ActivityType.Read)
            {
                action = "okudu";
            }

            if (ActivityType == ActivityType.Deleted)
            {
                action = "sildi.";
            }

            if (ActivityType == ActivityType.Updated)
            {
                action = "düzenledi.";
            }

            if (Subject == Subject.Project)
            {
                subject = "projesini";
            }

            if (Subject == Subject.Task)
            {
                subject = "görevini";
            }

            if (Subject == Subject.Announcement)
            {
                subject = "duyurusunu";
            }

            if (Subject == Subject.Document)
            {
                subject = "dökümanını";
            }

            if (Subject == Subject.Document)
            {
                subject = "dökümanını";
            }

            if (Subject == Subject.Meeting)
            {
                subject = "toplantısını";
            }

            string link = String.Format("<a href=\""+Url+"\">"+Title+"</a>");

            string activity = Username + " " + link + " " + subject + " " + action +". "+ FormatDate(DateCreated);

            TextInfo textInfo = new CultureInfo("tr-Tr", false).TextInfo;
            activity = textInfo.ToTitleCase(activity);

            return activity;
        }

        public static string FormatDate(DateTime dateTime)
        {

            var ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "bir saniye önce" : ts.Seconds + " saniye once";
            }
            if (delta < 120)
            {
                return "bir dakika önce";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " dakika önce";
            }
            if (delta < 5400) // 90 * 60
            {
                return "bir saat once";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " saat önce";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "Dün";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " gün önce";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "bir ay önce" : months + " ay önce";
            }
            int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "bir sene önce" : years + " sene önce";

        }
    }
}
