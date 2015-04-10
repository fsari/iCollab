using System;
using iCollab.Infra;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Model;
using Owin;

namespace iCollab
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {   
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                ExpireTimeSpan = TimeSpan.FromHours(30),
                SlidingExpiration = true,
                CookieName = "dev",
                Provider = new CookieAuthenticationProvider
                { 
                    
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });        
             
        }
    }
}