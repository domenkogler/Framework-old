using Shouldly;
using Xunit;

namespace Kogler.Framework.Test
{
    public partial class BootstrapperTests
    {
        public interface IView1 { }

        public class View1 : IView1 { }

        public class ViewModel1 : Screen<IView1> { }
        
        public class ViewModel11 : ViewModel1 { }

        public class View2 { }

        [View(typeof(View2))]
        public class ViewModel2 : Screen { }

        public class View3 { }

        [Navigation(typeof(View3), "/path")]
        public class ViewModel3 : Screen { }
        
        [Fact]
        public void ViewLocatorTest()
        {
            var func = Bootstrapper.LocateTypeForModelType;

            var view1 = func(typeof (ViewModel1), null, null);
            var view11 = func(typeof(ViewModel11), null, null);
            var view2 = func(typeof(ViewModel2), null, null);
            var view3 = func(typeof(ViewModel3), null, null);

            view1.ShouldNotBeNull();
            view11.ShouldNotBeNull();
            view2.ShouldNotBeNull();
            view3.ShouldNotBeNull();
        }
    }
}