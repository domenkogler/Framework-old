using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public class AutofacBootstrapper : Bootstrapper
    {
        public AutofacBootstrapper(bool useApplication) : base(useApplication)
        {
            Initialize();
        }

        protected IContainer Container { get; private set; }
        protected ContainerBuilder Builder { get; private set; }

        public ILifetimeScope GetLifetimeScope()
        {
            return Container.BeginLifetimeScope();
        }

        protected override void InitContainer()
        {
            Builder = new ContainerBuilder();
            ConfigureContainer(Builder);
        }

        protected virtual void ConfigureContainer(ContainerBuilder builder) { }
        
        protected sealed override void RegisterModules()
        {
            RegisterModules(Builder);
        }

        protected virtual void RegisterModules(ContainerBuilder builder)
        {
            var ass = AssemblySource.Instance.ToArray();
            builder.RegisterAssemblyTypes(ass)
                .Where(t => t.HasInterface<IModuleConfiguration>() || t.HasInterface<IPresentationConfiguration>())
                .AsImplementedInterfaces()
                .SingleInstance();
            builder.RegisterAssemblyModules(ass);
        }

        protected override void FinishContainer()
        {
            Container = Builder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;
            if (string.IsNullOrWhiteSpace(key))
            {
                if (Container.TryResolve(service, out instance)) return instance;
            }
            else
            {
                if (Container.TryResolveNamed(key, service, out instance)) return instance;
            }
            throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return (IEnumerable<object>) Container.Resolve(typeof (IEnumerable<>).MakeGenericType(service));
        }

        protected override void BuildUp(object instance)
        {
            Container.InjectProperties(instance);
        }

        protected override void DestroyContainer()
        {
            Container.Dispose();
        }
    }
}