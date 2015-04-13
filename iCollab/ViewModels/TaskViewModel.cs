﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Model;

namespace iCollab.ViewModels
{
    public class TaskViewModel
    {
        public TaskViewModel()
        {
            DateCreated = DateTime.Now;
        }

        public Guid Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(512)]
        [StringLength(512)]
        [Display(Name = "Başlık")]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "Tanımı")]
        public string Description { set; get; }

        [Display(Name = "Oluşturulma Zamanı")]
        public DateTime DateCreated{ get; set; }
 
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Başlangıç Zamanı")]
        public DateTime? StartDatetime { set; get; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Bitiş Zamanı")]
        public DateTime? EndDatetime { set; get; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Tamamlanma Zamanı")]
        public DateTime? DateCompleted { set; get; }

        [Display(Name = "Önceliği")]
        public Priority Priority { set; get; }

        [Display(Name = "Görev Durumu")]
        public TaskStatus TaskStatus { set; get; }

        [Display(Name = "Proje No")]
        public Guid ProjectId{ get; set; }
		
		[Display(Name = "Proje")]
        public Project Project { set; get; }

        public ProjectViewModel ProjectViewModel { get; set; } 

        public ICollection<Attachment> Attachments { get; set; }
        public List<Task> SubTasks { set; get; }
        public Task ParentTask { set; get; }
        public Guid? ParentTaskId { set; get; }
        public string CreatedBy { get; set; } 
        public string SelectedUserId { get; set; }
        public IEnumerable<SelectListItem> UserSelectList { get; set; }

        [Display(Name = "İşlendi mi?")]
        public bool IsProcessed { get; set; }

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

                if (TaskStatus == TaskStatus.Aktif && EndDatetime < DateTime.Now)
                {
                    return true;
                }

                return false;
            }
        }

        public int LateDays
        {
            get
            {
                if (EndDatetime.HasValue)
                {
                    return (DateTime.Now- EndDatetime.Value).Days;
                }

                return 0;
            }
        }
    }
}