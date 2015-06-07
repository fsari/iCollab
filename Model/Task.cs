using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Model
{
    public class Task : BaseEntity
    { 

        [StringLength(512)]
        [Display(Name = "Başlık")]
        [Required(ErrorMessage = "Zorunlu alan.")]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "İçerik")]
        public string Description { set; get; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
		[Display(Name = "Başlangıç Zamanı")]
        public DateTime Start { set; get; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
		[Display(Name = "Bitiş Zamanı")]
        public DateTime End { set; get; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
		[Display(Name = "Tamamlanma Zamanı")]
        public DateTime? DateCompleted { set; get; } 

        [Display(Name = "Önceliği")]
        public Priority Priority { set; get; }

        [Display(Name = "Görev Durumu")]
        public TaskStatus TaskStatus { set; get; }
        [Display(Name = "Proje No")]
        public Guid? ProjectId { get; set; }
        [Display(Name = "Proje")]
        public Project Project { get; set; } 
        public ICollection<Attachment> Attachments { get; set; } 
        public List<Task> SubTasks { set; get; }
        public ICollection<TaskUser> TaskUsers { set; get; }
        public Task ParentTask { set; get; }
        public Guid? ParentTaskId { set; get; }
        public decimal PercentComplete { get; set; }
        public int OrderId { get; set; }
        public TaskType TaskType { set; get; } 
        public string TaskOwnerId { set; get; }
        public ApplicationUser TaskOwner { set; get; }

        [NotMapped]
        public List<string> SelectedUsers { set; get; }

        [NotMapped]
        public bool IsLate {
            get
            {
                if (TaskStatus == TaskStatus.Aktif && End <  DateTime.Now)
                {
                    return true;
                }

                return false;
            }
        }

    }

    public enum TaskType
    {
        [Display(Name = "Genel")]
        General = 0,
        [Display(Name = "Geliştirme")]
        Development = 1,
        [Display(Name = "Bug")]
        Bug = 2,
        [Display(Name = "Değişiklik isteği")]
        ChangeRequest = 4,
        [Display(Name = "İyileştirme")]
        Enhancement = 8,
        [Display(Name = "Bakım")]
        Maintenance = 16,
        [Display(Name = "Q/A")]
        QualityAssurance = 32,
        [Display(Name = "Deploy")]
        Release = 64
    }
}
