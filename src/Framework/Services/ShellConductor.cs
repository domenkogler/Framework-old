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
            public object TopDock { get; internal set; }

            public object BottomDock { get; internal set; }

            public object LeftDock { get; internal set; }

            public object RightDock { get; internal set; }

            public void SetDock(object viewModel, Dock dock, IDictionary<string, string> state = null)
            {
                if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));
                var prop = GetType().GetProperty($"{dock}Dock");
                var oldVm = prop.GetValue(this, null);
                (oldVm as IDeactivate)?.Deactivate(true);
                (viewModel as IActivate)?.Activate();
                (viewModel as IHaveState)?.SetState(state);
                prop.SetValue(this, viewModel, null);
                NotifyOfPropertyChange(prop.Name);
            }
        }
    }
}