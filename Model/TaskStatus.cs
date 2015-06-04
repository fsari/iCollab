using System.ComponentModel.DataAnnotations;

namespace Model
{
    public enum TaskStatus
    {
        [Display(Name = "Planlandı")]
        Planlandı = 0,
        [Display(Name = "Aktif")]
        Aktif = 1,
        [Display(Name = "Tamamlandı")]
        Tamamlandı = 2,
        [Display(Name = "Durduruldu")]
        Durduruldu = 4,
        [Display(Name = "İptal")]
        Iptal = 8
    }
}
