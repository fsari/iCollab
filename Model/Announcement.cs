using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Model
{
    public class Announcement : BaseEntity 
    {

        public Announcement()
        {
            DateCreated = DateTime.Now;
        }

        [StringLength(512)]
        [Display(Name = "Başlık")]
        [Required]
        public string Title { get; set; }

        [AllowHtml]
        [DataType(DataType.Html)]
        [Display(Name = "İçerik")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Yayınlama tarihi")]
        public DateTime PublishDate{ get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Bitiş tarihi")]
        public DateTime EndDate { set; get; }

        public ICollection<Attachment> Attachments{ get; set; }
           
        public override int GetHashCode()
        { 
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var item = (Announcement) obj;

            if (item.Id == Id)
            {
                return true;
            }

            return false;
        }

        [NotMapped]
        public bool IsPublished
        {
            get
            {
                if (DateTime.Now < PublishDate)
                {
                    return true;
                }

                return false;
            }
        }

        [NotMapped]
        public bool IsExpired 
        {
            get
            {
                if (DateTime.Now > EndDate)
                {
                    return true;
                }

                return false;
            }
            
        }
    }
}
