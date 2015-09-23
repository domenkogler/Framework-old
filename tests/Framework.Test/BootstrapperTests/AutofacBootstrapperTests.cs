using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Caliburn.Micro;
using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
{
    public partial class BootstrapperTests
    {
        private class AutofacTestBootstrapper : AutofacBootstrapper
        {
            public AutofacTestBootstrapper() : base(false) { }

            protected override void ConfigureContainer(ContainerBuilder builder)
            {
                builder.RegisterAssemblyTypes(AssemblySource.Instance.ToArray())
                    .Where(t => t.HasInterface<IDependency1>() || t.HasInterface<IDependency2>())
                    .AsSelf()
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
        }

        public class AutofacBootstrapperTest
        {
            private readonly AutofacTestBootstrapper bootstrapper;

            public AutofacBootstrapperTest()
            {
                bootstrapper = new AutofacTestBootstrapper();
            }

            [Fact]
            public void ResolvesLazy()
            {
                using (var scope = bootstrapper.GetLifetimeScope())
                {
                    var dependency2 = scope.Resolve<Lazy<IDependency2>>();
                    dependency2.ShouldNotBeNull();
                    dependency2.Value.ShouldNotBeNull();
                    dependency2.Value.Dependency1.ShouldNotBeNull();
                }
            }

            [Fact]
            public void ResolveAllLazy()
            {
                using (var scope = bootstrapper.GetLifetimeScope())
                {
                    var modules = scope.Resolve<IEnumerable<Lazy<IModuleConfiguration>>>().ToArray();
                    var presentations = scope.Resolve<IEnumerable<Lazy<IPresentationConfiguration>>>().ToArray();

                    modules.ShouldNotBeNull();
                    modules.ShouldNotBeEmpty();

                    presentations.ShouldNotBeNull();
                    presentations.ShouldNotBeEmpty();
                }
            }
        }
    }
}