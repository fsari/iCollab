using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model;

namespace Core.Infrastructure
{
    public class ApplicationUserStore<TUser> : UserStore<TUser> where TUser : ApplicationUser
    {
        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }
}