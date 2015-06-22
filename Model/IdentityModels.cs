using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model.TodoList;

namespace Model
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        /*[Display(Name = "E-Posta")]
        public string EmailAddress { set; get; }*/
          
        [Display(Name = "Son Giriş")]
        public DateTime? LastLogin { set; get; }

        [Display(Name = "Oluşturulma tarihi")]
        public DateTime? DateCreated { set; get; }

        [Display(Name = "Resim")]
        public Attachment Picture { get; set; } 
        [Display(Name = "Adı Soyadı")]
        public string FullName { get; set; }
        [Display(Name = "Telefon")]
        public string Phone { set; get; }

        [Display(Name = "Birimi")]
        public Department Department{ set; get; }
        [Display(Name = "Birim Yoneticisi")]
        public bool IsDepartmentManager { set; get; }

        [NotMapped]
        public bool IsManager { get; set; }

        public bool Disabled { set; get; }

        public ICollection<Todo> Todos { set; get; }

        public string PasswordResetToken { set; get; }

        public bool PasswordResetTokenUsed { set; get; }
        public DateTime? PasswordTokenUsedOn { set; get; }
    }

    public class Department : BaseEntity
    {
        [Display(Name = "Adı Soyadı")]
        public string Name { set; get; }
    }
}