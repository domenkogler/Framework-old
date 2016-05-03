using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core.Resolving;
using Caliburn.Micro;
using Kogler.Framework.Autofac;
using NLog;
using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
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

        [Logger("Class attribute")]
        private class C
        {
            public C([Logger("param attr1", StackNames = true)] ILog log1, [Logger("param attr2")] ILogger log2, [Logger("")] Logger log3)
            {
                Log1 = log1;
                Log2 = log2;
                Log3 = log3;
            }

            public ILog Log1 { get; }
            public ILogger Log2 { get; }
            public Logger Log3 { get; }
        }

        [Fact]
        public void ScopeInjectionTest()
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
        public void FactoryScopeInjectionTest()
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

        [Fact]
        public void LoggerTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<LoggerModule>();
            builder.RegisterType<C>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var c = scope.Resolve<C>();
                ((Logger)c.Log1).Name.ShouldBe("Class attribute+param attr1");
                ((Logger)c.Log2).Name.ShouldBe("param attr2");
                c.Log3.Name.ShouldBe("C");
            }
        }
    }
}