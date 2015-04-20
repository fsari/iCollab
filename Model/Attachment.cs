using System;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Attachment :BaseEntity
    {  
        [Display(Name = "Adı")]
        public string Name { set; get; }  
        [Display(Name = "Yolu")]
        public string Path { get; set; } 
    }
}
