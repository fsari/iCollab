using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Model
{
    public class Document : BaseEntity
    {

        [Display(Name = "Başlık")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public ICollection<ContentPage> ContentPages { set; get; }

        public ApplicationUser Owner { set; get; }
        public string OwnerId { set; get; }

        public Guid ProjectId { set; get; }
        public Project Project { set; get; }

    }
}
