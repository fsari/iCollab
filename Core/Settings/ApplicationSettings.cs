using System.Configuration;

namespace Core.Settings
{

    public interface IApplicationSettings
    {
        bool CachingisEnabled { get; }
        bool EnableEmailer { get; }
        int IndexPageSize { get; }
        int PageSize { get; }
        string AnnouncementServerPath { get; }
        string AnnouncementAccessPath { get; }
        string DocumentsServerPath { get; }
        string DocumentsAccessPath { get; }
        string ProjectServerPath { get; }
        string ProjectAccessPath { get; }
        string TaskServerPath { get; }
        string TaskAccessPath { get; }
        string MeetingServerPath { get; }
        string MeetingAccessPath { get; }
        string ProfileServerPath { get; }
        string ProfileAccessPath { get; }
    }

    public class ApplicationSettings : IApplicationSettings
    {
        public bool CachingisEnabled
        {
            get
            {
                bool enableCaching;
                if (bool.TryParse(ConfigurationManager.AppSettings["CachingisEnabled"], out enableCaching))
                    return enableCaching;

                return true;
            }
        }

        public bool EnableEmailer
        {
            get
            {
                bool enableCaching;
                if (bool.TryParse(ConfigurationManager.AppSettings["EnableEmailer"], out enableCaching))
                    return enableCaching;

                return true;
            }
        }

        public int IndexPageSize
        {
            get
            {
                int indexPageSize;
                if (int.TryParse(ConfigurationManager.AppSettings["FrontEndPageSize"], out indexPageSize))
                    return indexPageSize;

                return 5;
            }
        }

        public int PageSize
        {
            get
            {
                int pagesize;
                if (int.TryParse(ConfigurationManager.AppSettings["PageSize"], out pagesize))
                    return pagesize;

                return 5;
            }
        }

        public string AnnouncementServerPath
        {
            get
            { 
                return ConfigurationManager.AppSettings["AnnouncementServerPath"]; 
            }
        }
        public string AnnouncementAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["AnnouncementAccessPath"];
            }
        }

        public string DocumentsServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DocumentsServerPath"];
            }
        }
        public string DocumentsAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DocumentsAccessPath"];
            }
        }

        public string ProjectServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectServerPath"];
            }
        }
        public string ProjectAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProjectAccessPath"];
            }
        }
        public string TaskServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["TaskServerPath"];
            }
        }
        public string TaskAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["TaskAccessPath"];
            }
        }
        public string MeetingServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MeetingServerPath"];
            }
        }
        public string MeetingAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["MeetingAccessPath"];
            }
        }

        public string ProfileServerPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProfileServerPath"];
            }
        }
        public string ProfileAccessPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProfileAccessPath"];
            }
        }
    }
}
