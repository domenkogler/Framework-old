namespace Kogler.Framework
{
    public abstract class AutofacModule : Autofac.Module, IModuleConfiguration
    {
        public virtual void Initialize() { }

        public virtual void Run() { }

        public virtual void Shutdown() { }
    }
}