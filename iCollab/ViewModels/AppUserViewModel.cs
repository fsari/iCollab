using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Model;
using Model.TodoList;

namespace iCollab.ViewModels
{
    public class AppUserViewModel
    {
        public string Id { set; get; }
        public string Email { set; get; }
        public string UserName { set; get; }


        [Display(Name = "Oluşturulma Zamanı")]
        public string DateCreated { set; get; }

        [Display(Name = "Son Giriş")]
        public DateTime? LastLogin { set; get; }

        [Display(Name = "Resim")]
        public virtual Attachment Picture { get; set; }
        [Display(Name = "Adı Soyadı")]
        public string FullName { get; set; }
        [Display(Name = "Telefon")]
        public string Phone { set; get; }

        [Display(Name = "Birimi")]
        public Department Department { set; get; }
        [Display(Name = "Birim Yoneticisi")]
        public bool IsDepartmentManager { set; get; }

        [NotMapped]
        public bool IsManager { get; set; }

        public virtual ICollection<Todo> Todos { set; get; }
    }
}