using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Model
{
    public class Document : BaseEntity
    { 
        public Document()
        {
            DateCreated = DateTime.Now;
        }

        [Display(Name = "Başlık")]
        [Required]
        public string Title{ get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description{ get; set; }
          
        public ICollection<Attachment> Attachments { get; set; }

        public ICollection<ContentPage> ContentPages { set; get; }

    }
}
