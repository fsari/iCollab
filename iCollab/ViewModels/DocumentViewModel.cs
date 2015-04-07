using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Model;

namespace iCollab.ViewModels
{
    public class DocumentViewModel
    { 
        public DocumentViewModel()
        {
            DateCreated = DateTime.Now;
        }

        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Oluşturulma Zamanı")]
        public DateTime DateCreated { set; get; }

        [Display(Name = "Başlık")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description { get; set; }

        public string UserCreated { set; get; }

        public ICollection<Attachment> Attachments { get; set; }
        public Guid ProjectId { set; get; }
    }
}