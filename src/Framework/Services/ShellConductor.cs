using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public interface IShellConductor : IConductActiveItem, IViewAware { }

    public interface IDockableShell : IShellConductor
    {
        void SetDock(object viewModel, Dock dock, IDictionary<string, string> state = null);
    }
    
    public abstract class ShellConductorBase<TViewModel> : Conductor<TViewModel>.Collection.OneActive, IShellViewModel, IShellConductor where TViewModel : class
    {
        public abstract class Dockable : ShellConductorBase<TViewModel>, IDockableShell
        {
            private object top;
            public object TopDock
            {
                get { return top; }
                set { top = value; NotifyOfPropertyChange(); }
            }

            private object bottom;
            public object BottomDock
            {
                get { return bottom; }
                set { bottom = value; NotifyOfPropertyChange(); }
            }

            private object left;
            public object LeftDock
            {
                get { return left; }
                set { left = value; NotifyOfPropertyChange(); }
            }

            private object right;
            public object RightDock
            {
                get { return right; }
                set { right = value; NotifyOfPropertyChange(); }
            }
        
            public void SetDock(object viewModel, Dock dock, IDictionary<string, string> state = null)
            {
                if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
                var prop = GetType().GetProperty($"{dock}Dock");
                var oldVm = prop.GetValue(this, null);
                (oldVm as IDeactivate)?.Deactivate(true);
                (viewModel as IActivate)?.Activate();
                (viewModel as IHaveState)?.SetState(state);
                prop.SetValue(this, viewModel, null);
            }
        }
    }
}