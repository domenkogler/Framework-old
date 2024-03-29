﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Shouldly;
using Xunit;
using Kogler.Framework.Mef;

namespace Kogler.Framework.Test
{
    public partial class BootstrapperTests
    {
        public class MefTestBootstrapper : Bootstrapper
        {
            public MefTestBootstrapper() : base(false, new Mef.Mef()) { }
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
            public IEnumerable<Lazy<IModule>> Modules { get; set; }

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
                    .GetExports<IModule>()
                    .ToArray();
                modules.ShouldNotBeNull();
                modules.ShouldNotBeEmpty();
                modules.Select(m => m.Value).ShouldBe(Modules.Select(m => m.Value));
            }
        }
    }
}