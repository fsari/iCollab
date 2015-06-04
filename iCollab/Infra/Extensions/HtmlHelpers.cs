using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Model;

namespace iCollab.Infra.Extensions
{
    public static class HtmlHelpers
    {
 
        public static string CssClass(this TaskStatus status)
        { 
            if (status == TaskStatus.Aktif)
            {
                return "label label-success";
            }

            if (status == TaskStatus.Tamamlandı)
            {
                return "label label-primary";
            }

            if (status == TaskStatus.Iptal)
            {
                return "label label-danger";
            }

            if (status == TaskStatus.Durduruldu)
            {
                return "label label-warning";
            } 
             
            return "label label-info";
        }

        public static string CssClass(this Priority priority)
        {
            if (priority == Priority.Acil)
            {
                return "label label-danger";
            }

            if (priority == Priority.Yüksek)
            {
                return "label label-warning";
            }

            if (priority == Priority.Normal)
            {
                return "label label-success";
            }

            if (priority == Priority.Düşük)
            {
                return "label label-info";
            }

            return "label label-info";
        }
    }
}