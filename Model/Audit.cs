using System;

namespace Model
{
    public class Audit
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string IpAddress { get; set; }
        public string UrlAccessed { get; set; }
        public DateTime Timestamp { get; set; }
        public string Data { get; set; }
        public string SessionId { get; set; }
    }
}
