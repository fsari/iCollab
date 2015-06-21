using System.ComponentModel.DataAnnotations;

namespace Model.Activity
{
    public enum Verb
    {
        [Display(Name="olusturdu",Description = "olusturdu")]
        Created=1,
        [Display(Name = "okudu", Description = "okudu")]
        Read=2,
        [Display(Name = "düzenledi", Description = "düzenledi")]
        Updated=3,
        [Display(Name = "sildi", Description = "sildi")]
        Deleted=4
    }
}
