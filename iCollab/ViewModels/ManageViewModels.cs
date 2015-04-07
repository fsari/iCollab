using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Model;

namespace iCollab.ViewModels
{
    public class ProfileViewModel
    { 
        public string PhoneNumber { get; set; }
        [Required]
        public string FullName { set; get; }

        public Attachment Attachment { set; get; }
    } 

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut şifreniz")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} en az {2} karakter olması.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni şifreniz")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni şifreniz")]
        [Compare("NewPassword", ErrorMessage = "Şifreniz uyuşmamaktadır. Kontrol ediniz.")]
        public string ConfirmPassword { get; set; }
    }
     
}