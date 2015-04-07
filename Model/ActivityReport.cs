using System.Collections.Generic;

namespace Model
{
    public class ActivityReport : BaseEntity
    {

        public ICollection<Attachment> Attachments { set; get; }
    }
}
