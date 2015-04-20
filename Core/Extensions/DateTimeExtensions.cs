using System;

namespace Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string FormatDate(this DateTime dateTime)
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