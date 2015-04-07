using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Model;

namespace iCollab.ViewModels
{
    public class AnnouncementViewModel
    {
        public AnnouncementViewModel()
        {
            DateCreated = DateTime.Now;
        }
         
        public ICollection<string> UserGroups { set; get; }

        public Guid Id { get; set; } 

        [DataType(DataType.DateTime)]
        [Display(Name = "Oluşturulma Zamanı")]
        public DateTime DateCreated { set; get; }

        [Required]
        [MinLength(3)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "Tanımı")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Yayınlama Zamanı")]
        public DateTime PublishDate { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Bitiş tarihi")]
        public DateTime EndDate { set; get; }

        public ICollection<Attachment> Attachments { get; set; }

        public string CreatedBy { set; get; }

        public string EditedBy { set; get; }
    }
}