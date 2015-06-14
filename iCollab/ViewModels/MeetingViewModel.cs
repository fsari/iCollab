using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace iCollab.ViewModels
{
    public class MeetingViewModel
    {

        public MeetingViewModel()
        {
            DateCreated = DateTime.Now;
        }

        public Guid Id { set; get; }

        [Display(Name = "Oluşturulma Zamanı")]
        public DateTime DateCreated { set; get; }

        [Required]
        [MinLength(3)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]

        [Display(Name = "İçerik")]
        public string Description { set; get; }

        [DataType(DataType.Date)] 
        [Display(Name = "Tarih")]
        public DateTime DateTime { set; get; }

        [Display(Name = "Saat")]
        public string Time { get; set; }

        [Required]
        [Display(Name = "Katılımcılar")]
        public string Attendees { set; get; }

        [Required]
        [Display(Name = "Lokasyon")]
        public string Location { get; set; }

        public Guid ProjectId { set; get; }

        public string CreatedBy { set; get; }

    }
}