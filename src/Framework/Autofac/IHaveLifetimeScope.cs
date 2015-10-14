using Autofac;

namespace Kogler.Framework.Autofac
{
    public interface IHaveLifetimeScope
    {
        ILifetimeScope Scope { get; }
    }
}