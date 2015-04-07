using System.ComponentModel.DataAnnotations;

namespace Model.Activity
{
    public enum ActivityType
    {
        [Display(Name="Olusturdu",Description = "Olusturdu")]
        Created,
        Read,
        Updated,
        Deleted
    }
}
