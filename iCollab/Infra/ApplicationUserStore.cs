using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Model;

namespace iCollab.Infra
{
    public class ApplicationUserStore<TUser> : UserStore<TUser> where TUser : ApplicationUser
    {
        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }
}