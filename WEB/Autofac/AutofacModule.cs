using Autofac;
using DataAccess.Services.Concrete;
using DataAccess.Services.Interface;


namespace WEB.Autofac
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(BaseRepository<>))
                   .As(typeof(IBaseRepository<>))
                   .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(BaseRepository<>).Assembly)
              .Where(t => t.Namespace != null
                       && t.Namespace.EndsWith("DataAccess.Services.Concrete")
                       && t.Name.EndsWith("Repository"))
              .AsImplementedInterfaces()
              .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(Business.Manager.Concrete.BlogPostManager).Assembly)
             .Where(t => t.Namespace != null && t.Namespace.EndsWith("Business.Manager.Concrete"))
             .AsImplementedInterfaces()
             .InstancePerLifetimeScope();

        }
    }
}
