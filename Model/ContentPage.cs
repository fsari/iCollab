using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Model
{
    public class ContentPage : BaseEntity
    {
        public ContentPage()
        {
            DateCreated = DateTime.Now;
        }

        [Display(Name = "Başlık")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description { get; set; } 
    }
}