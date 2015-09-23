using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
{
    public partial class BootstrapperTests
    {
        public class MefTestBootstrapper : MefBootstrapper
        {
            public MefTestBootstrapper() : base(false, new Mef()) { }
        }

        public class MefBootstrapperTest
        {
            private readonly MefTestBootstrapper bootstrapper;

            public MefBootstrapperTest()
            {
                bootstrapper = new MefTestBootstrapper();
                bootstrapper.Mef.Container.SatisfyImportsOnce(this);
            }

            [ImportMany]
            public IEnumerable<Lazy<IModuleConfiguration>> ModuleConfigurations { get; set; }
            [ImportMany]
            public IEnumerable<Lazy<IPresentationConfiguration>> PresentationConfigurations { get; set; }

            [Fact]
            public void ResolvesLazy()
            {
                var dependency2 = bootstrapper.Mef.Container.GetExport<IDependency2>();
                dependency2.ShouldNotBeNull();
                dependency2.Value.ShouldNotBeNull();
                dependency2.Value.Dependency1.ShouldNotBeNull();
            }

            [Fact]
            public void ResolveAllLazy()
            {
                var modules = bootstrapper.Mef.Container
                    .GetExports<IModuleConfiguration>()
                    .ToArray();
                var presentations = bootstrapper.Mef.Container
                    .GetExports<IPresentationConfiguration>()
                    .ToArray();
                modules.ShouldNotBeNull();
                modules.ShouldNotBeEmpty();
                modules.Select(m => m.Value).ShouldBe(ModuleConfigurations.Select(m => m.Value));
                
                presentations.ShouldNotBeNull();
                presentations.ShouldNotBeEmpty();
                presentations.Select(m => m.Value).ShouldBe(PresentationConfigurations.Select(m => m.Value));
            }
        }
    }
}