using System;
using System.IO;
using System.Web.Hosting;

namespace iCollab.Infra.Extensions
{
    public static class AttachmentHelper
    {
        public static string CreateGuidFilename(string file)
        {
            var guidName = Guid.NewGuid().ToString();

            var extension = Path.GetExtension(file);

            var newName = String.Format("{0}{1}", guidName, extension);

            return newName;
        }

        public static string GetDatetimePath()
        {
            var daypart = DateTime.Now.Day.ToString();
            var monthpart = DateTime.Now.Month.ToString();
            var yearpart = DateTime.Now.Year.ToString();

            var folderPath = Path.Combine(yearpart, monthpart, daypart).Replace('\\', '/')+"/";

            return folderPath;
        }

        public static string GetUploadPath(string filename, string serverPath)
        {
            return HostingEnvironment.MapPath(string.Concat(string.Concat(serverPath, GetDatetimePath()), filename)); ;
        }

        public static string GetAccessPath(string filename, string accessPath)
        {
            return string.Format("{0}{1}", string.Concat(accessPath, GetDatetimePath()), filename);
        }
    }
}