using System.ComponentModel.DataAnnotations;

namespace Model
{
    public enum Priority
    {
        [Display(Name = "Normal")]
        Normal = 0,
        [Display(Name = "Düşük")]
        Düşük = 1,
        [Display(Name = "Yüksek")]
        Yüksek = 2,
        [Display(Name = "Acil")]
        Acil = 4
    }
}
