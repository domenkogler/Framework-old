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

        [Export(typeof(IModule))]
        public class Module1 : Module
        {
            [ImportingConstructor]
            public Module1(IDependency2 dependency2)
            {
                Dependency2 = dependency2;
            }
            private IDependency2 Dependency2 {get;}
        }

        [Export(typeof(IModule))]
        public class Module2 : Module
        {
            [ImportingConstructor]
            public Module2(IDependency1 dependency1)
            {
                Dependency1 = dependency1;
            }
            private IDependency1 Dependency1 { get; }
        }
    }
}