using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Model
{
    public class Meeting : BaseEntity
    {   
        [MinLength(3)]
        [Display(Name = "Başlık")]
        [Required(ErrorMessage = "Zorunlu alan.")]
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
        [Display(Name = "Katılımcılar")]
        [Required(ErrorMessage = "Zorunlu alan.")]
        public string Attendees { set; get; } 
        [Display(Name = "Lokasyon")]
        [Required(ErrorMessage = "Zorunlu alan.")]
        public string Location{ get; set; } 
        public Guid? ProjectId { set; get; } 
        public Project Project { set; get; } 
        public ICollection<Attachment> Attachments { get; set; } 
        public bool IsPublic { set; get; } 
        public string OwnerId { set; get; }
        public ApplicationUser Owner { set; get; }
          
    }
}
