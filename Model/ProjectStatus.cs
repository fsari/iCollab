using System.ComponentModel.DataAnnotations;

namespace Model
{
    public enum ProjectStatus
    {
        [Display(Name = "Planlandı")]
        Planlandı = 0,
        [Display(Name = "Aktif")]
        Aktif = 1,
        [Display(Name = "Bitti")]
        Bitti = 2,
        [Display(Name = "Beklemede")]
        Beklemede = 4,
        [Display(Name = "Iptal")]
        Iptal = 8, 
    }
}