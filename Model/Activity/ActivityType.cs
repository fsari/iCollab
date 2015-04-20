using System.ComponentModel.DataAnnotations;

namespace Model.Activity
{
    public enum Verb
    {
        [Display(Name="olusturdu",Description = "olusturdu")]
        Created=0,
        [Display(Name = "okudu", Description = "okudu")]
        Read=1,
        [Display(Name = "düzenledi", Description = "düzenledi")]
        Updated=2,
        [Display(Name = "sildi", Description = "sildi")]
        Deleted=3
    }
}
