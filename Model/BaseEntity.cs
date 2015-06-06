using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model
{
    public abstract class BaseEntity : IEquatable<BaseEntity>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        public Guid Id { get; set; }
          
        [Index]
        [Display(Name = "Silinmiş mi?")]
        public bool IsDeleted { get; set; }
        [Display(Name = "Oluşturulma Zamanı")]
        public DateTime DateCreated { set; get; }
        [Display(Name = "Düzenleme Zamanı")]
        public DateTime? DateEdited { set; get; }
        [Display(Name = "Silinme Zamanı")]
        public DateTime? DateDeleted { set; get; } 
        [Column(TypeName = "VARCHAR")]
        [StringLength(200)]
        [Index]
        public string CreatedBy { set; get; }
        [Display(Name = "Düzenliyen")]
        public string EditedBy { set; get; }
        [Display(Name = "Silen")]
        public string DeletedBy { set; get; }

        public bool Equals(BaseEntity other)
        {
            if (other.Id == Id)
            {
                return true;
            }

            return false;
        }
    }
}
