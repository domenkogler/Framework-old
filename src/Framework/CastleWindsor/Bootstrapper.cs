using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

namespace Kogler.Framework.Castle
{
    public static class WindsorExtensions
    {
        public static IWindsorContainer Register(this IWindsorContainer container, IEnumerable<Assembly> assemblies, params Func<Assembly, IRegistration>[] regitrations)
        {
            assemblies
                .SelectMany(a => regitrations.Select(r => r(a)))
                .Apply(r => container.Register(r));
            return container;
        }
    }

    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper(bool useApplication) : base(useApplication)
        {
            Initialize();
        }

        protected WindsorContainer Container { get; } = new WindsorContainer();
        
        protected override void InitContainer()
        {
            Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(Container.Kernel, true));
            Container.AddFacility<TypedFactoryFacility>();
            Container.Register(Component.For<IWindsorContainer>().Instance(Container).LifestyleSingleton());
            Container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());
            ConfigureContainer();
        }

        protected virtual void ConfigureContainer() { }

        protected override void RegisterModules()
        {
            Container.Register(AssemblySource.Instance,
                ass => Classes.FromAssembly(ass).BasedOn<IModule>().WithServiceAllInterfaces().LifestyleSingleton());
            ConfigureContainer();
        }

        protected override object GetInstance(Type service, string key)
        {
            return string.IsNullOrWhiteSpace(key)
                ? Container.Resolve(service)
                : Container.Resolve(key, service);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return (IEnumerable<object>)Container.ResolveAll(service);
        }

        protected override void BuildUp(object instance)
        {
            instance.GetType().GetProperties()
                .Where(property => property.CanWrite && property.PropertyType.IsPublic)
                .Where(property => Container.Kernel.HasComponent(property.PropertyType))
                .ForEach(property => property.SetValue(instance, Container.Resolve(property.PropertyType), null));
        }

        protected override void DestroyContainer()
        {
            Container.Dispose();
        }
    }
}