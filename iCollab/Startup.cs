using iCollab.Infra.Extensions;
using Microsoft.Owin; 
using Owin; 

[assembly: OwinStartupAttribute(typeof(iCollab.Startup))]
namespace iCollab
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        { 
            ConfigureAuth(app);
            //app.UseSimpleLogger();
        }
    }
}
