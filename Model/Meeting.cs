using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Model
{
    public class Meeting : BaseEntity
    { 

        [Required]
        [MinLength(3)]
        [Display(Name = "Başlık")]
        public string Title{ get; set; }
         
        [AllowHtml]
        [DataType(DataType.Html)]

        [Display(Name = "İçerik")]
        public string Description { set; get; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Tarih ve zaman")]
        public DateTime DateTime{ set; get; } 

        [Required]
        [Display(Name = "Katılımcılar")]
        public string Attendees { set; get; }
         
        [Required]
        [Display(Name = "Lokasyon")]
        public string Location{ get; set; }

        public ICollection<Attachment> Attachments { get; set; }
          
    }
}
