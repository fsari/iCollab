using System;
using System.Data.Entity;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Core.Caching;
using Core.Mappers;
using Core.Repository;
using Core.Service;
using Core.Service.CrudService;
using Core.Settings;
using iCollab.Infra; 
using MemoryCacheT;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Model;

namespace iCollab
{
    public class DependencyRegistrar
    {
        public static void RegisterDependencies()
        {  
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).AsImplementedInterfaces();

            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterType<ApplicationSettings>().As<IApplicationSettings>();

            DataContext db = new DataContext();

            builder.RegisterType<DataContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<UoW>().AsSelf().InstancePerRequest();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerRequest();

            builder.RegisterGeneric(typeof(CacheManager<,>)).As(typeof(ICacheManager<,>)).SingleInstance();
            //builder.RegisterType<AppMailer>().As<IAppMailer>().InstancePerRequest();

            TimeSpan expiration = TimeSpan.FromHours(1);

            builder.RegisterGeneric(typeof(Cache<,>)).As(typeof(ICache<,>)).WithParameter("timerInterval", expiration).SingleInstance();

            builder.RegisterGeneric(typeof(Mapper<,>)).As(typeof(IMapper<,>)).InstancePerRequest();

            builder.RegisterGeneric(typeof(BaseCrudService<>)).As(typeof(ICrudService<>)).InstancePerRequest();

            builder.RegisterType<DocumentService>().As<IDocumentService>().InstancePerRequest();
            builder.RegisterType<AnnouncementService>().As<IAnnouncementService>().InstancePerRequest();
            builder.RegisterType<MeetingService>().As<IMeetingService>().InstancePerRequest();
            builder.RegisterType<TaskService>().As<ITaskService>().InstancePerRequest();
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerRequest(); 
            builder.RegisterType<AttachmentServıce>().As<IAttachmentService>().InstancePerRequest();

            /*builder.RegisterType<ApplicationUserStore<ApplicationUser>>().As<IUserStore<ApplicationUser>>();*/

            //builder.RegisterType<UserManager<ApplicationUser>>().UsingConstructor(typeof(IUserStore<ApplicationUser>)).InstancePerDependency();

            //builder.RegisterType<UserManager<ApplicationUser>>().UsingConstructor(typeof(IUserStore<ApplicationUser>)).InstancePerDependency();

           /* builder.Register(c => new UserStore<ApplicationUser>(db)).AsImplementedInterfaces();

            builder.RegisterType<ApplicationUserManager>().AsSelf();
          
            builder.RegisterType<ApplicationUserManager>().As<UserManager<ApplicationUser>>().InstancePerRequest();

            builder.Register(c => new IdentityFactoryOptions<ApplicationUserManager>
            {
                DataProtectionProvider = new Microsoft.Owin.Security.DataProtection.DpapiDataProtectionProvider("iCollab​")
            });

            builder.RegisterType<ApplicationSignInManager>().AsSelf();*/

            builder.RegisterType<ApplicationUserStore<ApplicationUser>>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerLifetimeScope();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).AsSelf(); 
 
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));  
        }
         
    }
}