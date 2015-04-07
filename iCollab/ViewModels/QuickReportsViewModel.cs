using System.ComponentModel.DataAnnotations;

namespace iCollab.ViewModels
{
    public class QuickReportsViewModel
    {
        [Display(Name = "Proje Sayısı")]
        public int ProjectsCount{ get; set; }
        [Display(Name = "Görev Sayısı")]
        public int TasskCount{ get; set; }
        [Display(Name = "Kullanıcı Sayısı")]
        public int UsersCount{ get; set; }
        [Display(Name = "Duyuru Sayısı")]
        public int AnnouncementsCount{ get; set; }
        [Display(Name = "Kullanıcı Grubu Sayısı")]
        public int UserGroupsCount { get; set; }

        public int MeetingCount{ get; set; }
    }
}