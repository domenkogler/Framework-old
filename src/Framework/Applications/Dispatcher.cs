using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Kogler.Framework
{
    public static class Dispatcher
    {
        /// <summary>
        /// Execute the event queue of the dispatcher.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            System.Windows.Threading.Dispatcher.CurrentDispatcher
                .BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            System.Windows.Threading.Dispatcher.PushFrame(frame);
        }

        private static object ExitFrame(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
        
        public static void BeginInvoke(Action action)
        {
            Current.BeginInvoke(action);
        }

        public static System.Windows.Threading.Dispatcher Current => System.Windows.Threading.Dispatcher.CurrentDispatcher; 

        public static bool IsDispatcherSynchronizationContext => SynchronizationContext.Current is DispatcherSynchronizationContext;

        public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
