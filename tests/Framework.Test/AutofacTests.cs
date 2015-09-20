using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Autofac;
using Autofac.Core.Resolving;
using Shouldly;
using Xunit;

namespace Framework.Test
{
    public class AutofacTests
    {
        private class A
        {
            public A() { }
            public A(B b, IComponentContext context)
            {
                B = b;
                Context = context;
            }

            public B B { get; }

            public IComponentContext Context { get; }
        }

        private class B
        {
            public B(IEnumerable<ILifetimeScope> scopes)
            {
                Scopes = scopes;
            }

            public IEnumerable<ILifetimeScope> Scopes { get; }
        }

        [Fact]
        public void TestScopeInjection()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<A>().InstancePerLifetimeScope();
            builder.RegisterType<B>().InstancePerLifetimeScope();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var a = scope.Resolve<A>();
                scope.ShouldBe(a.B.Scopes.First());
                scope.ShouldBe(a.Context);
                var b = scope.Resolve<B>();
                a.B.ShouldBe(b);
            }
        }

        [Fact]
        public void TestFactoryScopeInjection()
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new A(c.Resolve<B>(),c)).InstancePerLifetimeScope();
            builder.RegisterType<B>().InstancePerLifetimeScope();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var a = scope.Resolve<A>();
                scope.ShouldBe(a.B.Scopes.First());
                scope.ShouldBe(((IInstanceLookup) a.Context).ActivationScope);
                var b = scope.Resolve<B>();
                a.B.ShouldBe(b);
            }
        }
    }
}