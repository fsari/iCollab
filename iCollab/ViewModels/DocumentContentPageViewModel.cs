using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace iCollab.ViewModels
{
    public class DocumentContentPageViewModel
    {
        public Guid DocumentGuid { set; get; }

        public Guid Id { set; get; }

        [Display(Name = "Başlık")]
        [Required]
        public string Title { get; set; }

        [Display(Name = "İçerik")]
        [AllowHtml]
        public string Description { get; set; } 
    }
}