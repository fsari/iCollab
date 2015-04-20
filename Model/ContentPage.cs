using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Model
{
    public class ContentPage : BaseEntity
    { 

        [Display(Name = "Başlık")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description { get; set; } 
    }
}