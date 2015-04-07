using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Attachment :BaseEntity
    {
        public Attachment()
        {
            DateCreated = DateTime.Now;
        }
         
        [Display(Name = "Adı")]
        public string Name { set; get; }  
        [Display(Name = "Yolu")]
        public string Path { get; set; } 
    }
}
