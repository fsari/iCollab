using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
using System.Web.Mvc;

namespace Model
{
    public class Project : BaseEntity
    { 
         
        [StringLength(512)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }
        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "İçerik")]
        public string Description { set; get; }
        [Display(Name = "Başlangıç Zamanı")]
        public DateTime? StartDatetime { set; get; }
        [Display(Name = "Bitiş Zamanı")]
        public DateTime? EndDatetime { set; get; }
        [Display(Name = "Durumu")]
        public ProjectStatus Status { set; get; }
        [Display(Name = "Önceliği")]
        public Priority Priority { set; get; }

        [Display(Name="Tamamlanma Tarihi")]
        public DateTime? DateCompleted { set; get; }
        [Display(Name = "İptal Tarihi")]
        public DateTime? DateCancelled { set; get; }
           
        public string ProjectOwner { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<Document> Documents { set; get; }
        public ICollection<Attachment> Attachments { set; get; }
        public ICollection<Meeting> Meetings { set; get; }
        public ICollection<ProjectUsers> ProjectUsers { set; get; } 

    }
}
