using System;
using System.Diagnostics;
using System.Linq;
using Caliburn.Micro;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
{
    public partial class BootstrapperTests
    {
        public class WindsorTestBootstrapper : WindsorBootstrapper
        {
            public WindsorTestBootstrapper() : base(false) { }

            protected override void ConfigureContainer()
            {
                Container.Register(Classes.FromAssembly(AssemblySource.Instance.First())
                    .BasedOn(typeof (IDependency1), typeof (IDependency2))
                    .WithServiceAllInterfaces()
                    .WithServiceSelf()
                    .LifestyleTransient());
            }

            private void WhatDoIHave()
            {
                Trace.WriteLine("Container has only these components:");
                foreach (var definition in Container.Kernel.GetAssignableHandlers(typeof(object)))
                {
                    Trace.WriteLine(definition.ToString());
                }
            }

            public T Get<T>()
            {
                try
                {
                    return Container.Resolve<T>();
                }
                catch (Exception)
                {
                    WhatDoIHave();
                    throw;
                }
            }

            public T[] GetAll<T>()
            {
                try
                {
                    return Container.ResolveAll<T>();
                }
                catch (Exception)
                {
                    WhatDoIHave();
                    throw;
                }
            }
        }

        public class WindsorBootstrapperTest
        {
            private readonly WindsorTestBootstrapper bootstrapper;

            public WindsorBootstrapperTest()
            {
                bootstrapper = new WindsorTestBootstrapper();
            }

            [Fact]
            public void ResolvesLazy()
            {
                var dependency2 = bootstrapper.Get<Lazy<IDependency2>>();
                dependency2.ShouldNotBeNull();
                dependency2.Value.ShouldNotBeNull();
                dependency2.Value.Dependency1.ShouldNotBeNull();
            }

            //[Fact(Skip = "Windsor cannot resolve many Lazy<>")]
            //public void ResolveAllLazy()
            //{
            //    var modules = bootstrapper.GetAll<Lazy<IModule>>().ToArray();
            //    modules.ShouldNotBeNull();
            //    modules.ShouldNotBeEmpty();
            //}
        }
    }
}