using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Core.Caching;
using Core.Logging;
using Core.Mappers;
using Core.Repository;
using Core.Service;
using Core.Service.CrudService;
using Core.Settings;
using iCollab.Infra;
using MemoryCacheT;
using Microsoft.Owin.Security;
using Model;
using Serilog;
using ILogger = Core.Logging.ILogger;

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

            builder.RegisterModule(new CachingModule());

            builder.RegisterType<ApplicationSettings>().As<IApplicationSettings>();

            builder.RegisterType<DataContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<UoW>().AsSelf().InstancePerRequest();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();

            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerRequest();


            //builder.RegisterType<AppMailer>().As<IAppMailer>().InstancePerRequest();


            builder.RegisterGeneric(typeof(Mapper<,>)).As(typeof(IMapper<,>)).InstancePerRequest();

            builder.RegisterGeneric(typeof(BaseCrudService<>)).As(typeof(ICrudService<>)).InstancePerRequest();

            builder.RegisterType<DocumentService>().As<IDocumentService>().InstancePerRequest();
            builder.RegisterType<MeetingService>().As<IMeetingService>().InstancePerRequest();
            builder.RegisterType<TaskService>().As<ITaskService>().InstancePerRequest();
            builder.RegisterType<ProjectService>().As<IProjectService>().InstancePerRequest();
            builder.RegisterType<AttachmentService>().As<IAttachmentService>().InstancePerRequest();
            builder.RegisterType<DependencyService>().As<IDependencyService>().InstancePerRequest();

            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerRequest();

            builder.RegisterGeneric(typeof(ActivityService<>)).As(typeof(IActivityService<>)).InstancePerRequest();

            builder.Register<Serilog.ILogger>((c, p) =>
            {
                return new LoggerConfiguration().ReadFrom.AppSettings().WriteTo.RollingFile(AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "/Log-{Date}.txt").CreateLogger();
            }).SingleInstance();

            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();


            builder.RegisterType<ApplicationUserStore<ApplicationUser>>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationSignInManager>().AsSelf().InstancePerLifetimeScope();
            builder.Register<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication).AsSelf();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }



    }

    public class CachingModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<StaticCache>().Keyed<ICache>(typeof(StaticCache)).SingleInstance();
            builder.RegisterType<AspNetCache>().Keyed<ICache>(typeof(AspNetCache)).SingleInstance();
            builder.RegisterType<RequestCache>().Keyed<ICache>(typeof(RequestCache)).InstancePerRequest();

            builder.RegisterType<CacheManager<RequestCache>>()
                .As<ICacheManager>()
                .InstancePerRequest();
            builder.RegisterType<CacheManager<StaticCache>>()
                .Named<ICacheManager>("static")
                .SingleInstance();
            builder.RegisterType<CacheManager<AspNetCache>>()
                .Named<ICacheManager>("aspnet")
                .SingleInstance();

            // Register resolving delegate
            builder.Register<Func<Type, ICache>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return keyed => cc.ResolveKeyed<ICache>(keyed);
            });

            builder.Register<Func<string, ICacheManager>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<ICacheManager>(named);
            });

            builder.Register<Func<string, Lazy<ICacheManager>>>(c =>
            {
                var cc = c.Resolve<IComponentContext>();
                return named => cc.ResolveNamed<Lazy<ICacheManager>>(named);
            });
        }
    }
}