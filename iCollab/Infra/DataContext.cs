using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Model;
using Model.Activity;
using Model.TodoList;

namespace iCollab.Infra
{
    public class DataContext : IdentityDbContext<ApplicationUser>
    {
        public DataContext() : base("DefaultConnection")
        {
        }

        public IDbSet<Todo> Todos { set; get; }
        public IDbSet<Project> Projects { set; get; }
        public IDbSet<Task> Tasks { set; get; }
        public IDbSet<Announcement> Announcements { get; set; } 
        public IDbSet<Meeting> Meetings { get; set; }
        public IDbSet<Document> Documents { get; set; }
        public IDbSet<ContentPage> ContentPages { set; get; }
        public IDbSet<Activity> Activities { set; get; }
        public IDbSet<Dependency> Dependencies{ set; get; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    string error = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    var sb = new StringBuilder();

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        string local = string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName,
                            ve.ErrorMessage);

                        sb.Append(local);
                    }
                }
                throw;
            }
        }
         
    }
}