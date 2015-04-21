using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Model;

namespace iCollab.ViewModels
{
    public class ProjectViewModel
    {
        public ProjectViewModel()
        {
            DateCreated = DateTime.Now;
        }

        public double Progress()
        {
            if (Tasks == null)
            {
                return 0;
            }

            var totalTasks = Tasks.Count;

            if (totalTasks == 0)
            {
                return 0;
            }

            var completedTasks = Tasks.Count(x => x.TaskStatus == TaskStatus.Tamamlandı);

            var percentage = (completedTasks*100) / totalTasks;

            return percentage;
        }

        public Guid Id { get; set; }
        [Display(Name = "Oluşturulma Zamanı")]
        public DateTime DateCreated { set; get; }

        [Required]
        [MinLength(3)]
        [MaxLength(512)]
        [Display(Name = "Başlık")] 
        public string Title{ get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "Tanımı")]
        public string Description { set; get; } 

        [Display(Name = "Başlangıç Tarihi")]
        public DateTime? StartDatetime { set; get; }

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDatetime { set; get; }

        [Display(Name = "Durumu")]
        public ProjectStatus Status { set; get; }

        [Display(Name = "Önceliği")]
        public Priority Priority { set; get; }

        [Display(Name = "Oluşturan")]
        public AppUserViewModel CreatedBy { set; get; }
        [Display(Name = "Oluşturan")]
        public string ProjectOwner { get; set; }

        [Display(Name = "Sonraki Proje")]
        public Project NextProject { get; set; }

        [Display(Name = "Önceki Proje")]
        public Project PreviousProject { set; get; }

        public ICollection<Task> Tasks { get; set; }
        public ICollection<Document> Documents{ get; set; }
        public ICollection<Meeting> Meetings { set; get; }
        public ICollection<Attachment> Attachments { set; get; }
        [Display(Name = "Tamamlanma Yüzdesi")]
        public int PercentComplete{ get; set; }

        public IEnumerable<UserSelectViewModel> SelectedProjectUsers { set; get; } 
        public List<string> SelectedUsers { set; get; }

        public bool IsLate
        {
            get
            { 
                if (EndDatetime.HasValue == false)
                {
                    return false;
                }

                if (Status == ProjectStatus.Aktif && EndDatetime < DateTime.Now)
                {
                    return true;
                }

                return false;
            }
        }
    }

    public class UserSelectViewModel
    {
        public string FullName { get; set; }
        public string Id { get; set; }
    }
}