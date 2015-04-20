using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Model.Activity
{

    public interface IActivity
    {
        void HandleActivity(ApplicationUser user, Verb verb, object obj);
    }

    public static class ActivityExtentions
    {
        public static void Created(this Activity activity, ApplicationUser user)
        {
            activity.HandleActivity(user, Verb.Created, null);
        }
    }

    public class Activity : BaseEntity, IActivity
    {  
        public ApplicationUser User{ get; set; }
        public Verb Verb { get; set; }

        public void HandleActivity(ApplicationUser user, Verb verb, object obj)
        {
            throw new NotImplementedException();
        }
         
        /*public override string ToString()
        {
            string action = string.Empty;
            string subject = string.Empty;

            if (Verb == Verb.Created)
            {
                action = "oluşturdu";
            }

            if (Verb == Verb.Read)
            {
                action = "okudu";
            }

            if (Verb == Verb.Deleted)
            {
                action = "sildi.";
            }

            if (Verb == Verb.Updated)
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
        } */
         
         
    }
}
