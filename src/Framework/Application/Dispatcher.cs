using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void BeginOnUIThread(this Action action)
        {
            Current.BeginInvoke(action);
        }

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns></returns>
        public static Task OnUIThreadAsync(this Action action)
        {
            var taskSource = new TaskCompletionSource<object>();
            Action method = () => {
                try
                {
                    action();
                    taskSource.SetResult(null);
                }
                catch (Exception ex)
                {
                    taskSource.SetException(ex);
                }
            };
            Current.BeginInvoke(method);
            return taskSource.Task;
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="NotImplementedException"></exception>
        public static void OnUIThread(this Action action)
        {
            Exception exception = null;
            Action method = () => {
                try { action(); }
                catch (Exception ex) { exception = ex; }
            };
            Current.Invoke(method);
            if (exception != null) throw new TargetInvocationException("An error occurred while dispatching a call to the UI Thread", exception);
        }
        
        /// <summary>
        /// Executes the handler immediately if the element is loaded, otherwise wires it to the Loaded event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>true if the handler was executed immediately; false otherwise</returns>
        public static bool OnLoad(this FrameworkElement element, RoutedEventHandler handler)
        {
            if (element == null) return false;
            if (element.IsLoaded)
            {
                handler(element, new RoutedEventArgs());
                return true;
            }
            RoutedEventHandler loaded = null;
            loaded = (s, e) => {
                element.Loaded -= loaded;
                handler(s, e);
            };
            element.Loaded += loaded;
            return false;
        }

        /// <summary>
        /// Executes the handler when the element is unloaded.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void OnUnload(this FrameworkElement element, RoutedEventHandler handler)
        {
            if (element == null) return;
            RoutedEventHandler unloaded = null;
            unloaded = (s, e) => {
                element.Unloaded -= unloaded;
                handler(s, e);
            };
            element.Unloaded += unloaded;
        }


        /// <summary>
        /// Executes the handler the next time the elements's LayoutUpdated event fires.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void OnLayoutUpdated(this FrameworkElement element, EventHandler handler)
        {
            if (element == null) return;
            EventHandler onLayoutUpdate = null;
            onLayoutUpdate = (s, e) => {
                element.LayoutUpdated -= onLayoutUpdate;
                handler(element, e);
            };
            element.LayoutUpdated += onLayoutUpdate;
        }


        public static System.Windows.Threading.Dispatcher Current => System.Windows.Threading.Dispatcher.CurrentDispatcher; 

        public static bool IsDispatcherSynchronizationContext => SynchronizationContext.Current is DispatcherSynchronizationContext;

        public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());
    }
}
