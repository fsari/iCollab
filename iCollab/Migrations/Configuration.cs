using System.Collections.ObjectModel;
using iCollab.Infra;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Model;

namespace iCollab.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<iCollab.Infra.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }



        protected override void Seed(iCollab.Infra.DataContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var user = userManager.FindByEmail("mrdiesel@gmail.com");

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "mrdiesel@gmail.com", 
                    FullName = "serkan atagun",
                    Email = "mrdiesel@gmail.com"
                };

                userManager.Create(user, "atagun");

                userManager.AddToRole(user.Id, "manager");
            }

            if (!roleManager.RoleExists("manager"))
            {
                var role = new IdentityRole { Name = "manager", Id = Guid.NewGuid().ToString()};
                roleManager.Create(role);

                var userRole = new IdentityRole { Name = "user", Id =  Guid.NewGuid().ToString()};
                roleManager.Create(userRole);

            }

            user.Claims.Add(new IdentityUserClaim() { ClaimType = "name", ClaimValue = "mrdiesel@gmail.com" });
            user.Claims.Add(new IdentityUserClaim() { ClaimType = "role", ClaimValue = "IdentityManagerAdministrator" }); 


            /*                            Claims = new Claim[]{
                        new Claim(Constants.ClaimTypes.Name, "Admin"),
                        new Claim(Constants.ClaimTypes.Role, "IdentityManagerAdministrator"),
                    }*/

            var userInRole = userManager.IsInRole(user.Id, "manager");

            if (userInRole == false)
            {
                userManager.AddToRole(user.Id, "manager");
            }
        }
    }
}
