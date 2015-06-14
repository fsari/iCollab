using System.Collections.Generic;
using Model;

namespace iCollab.ViewModels
{
    public class AttachmentViewModel
    {
        public IEnumerable<Attachment> Attachments { set; get; }
        public string RemovePath { set; get; }
        public string UploadPath { set; get; }

        public bool CanUserUpload { set; get; }
    }
}