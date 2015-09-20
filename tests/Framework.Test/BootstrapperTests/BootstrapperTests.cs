using System.ComponentModel.Composition;

namespace Kogler.Framework.Test
{
    public partial class BootstrapperTests
    {
        public interface IDependency1 { }
        [Export(typeof(IDependency1)), Export]
        public class Dependency1 : IDependency1 { }

        public interface IDependency2
        {
            IDependency1 Dependency1 { get; }
        }
        [Export(typeof(IDependency2)), Export]
        public class Dependency2 : IDependency2
        {
            [ImportingConstructor]
            public Dependency2(Dependency1 dependency1)
            {
                Dependency1 = dependency1;
            }

            public IDependency1 Dependency1 { get; }
        }

        [Export(typeof(IModuleConfiguration))]
        public class Module1 : IModuleConfiguration
        {
            [ImportingConstructor]
            public Module1(IDependency2 dependency2)
            {
                Dependency2 = dependency2;
            }
            private IDependency2 Dependency2 {get;}
            public void Initialize() { }

            public void Run() { }

            public void Shutdown() { }
        }

        [Export(typeof(IPresentationConfiguration))]
        public class PresentationModule1 : IPresentationConfiguration
        {
            public void Initialize() { }
        }

        [Export(typeof(IModuleConfiguration))]
        public class Module2 : IModuleConfiguration
        {
            [ImportingConstructor]
            public Module2(IDependency1 dependency1)
            {
                Dependency1 = dependency1;
            }
            private IDependency1 Dependency1 { get; }
            public void Initialize() { }

            public void Run() { }

            public void Shutdown() { }
        }

        [Export(typeof(IPresentationConfiguration))]
        public class PresentationModule2 : IPresentationConfiguration
        {
            public void Initialize() { }
        }
    }
}