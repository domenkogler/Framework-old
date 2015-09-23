using Autofac;

namespace Kogler.Framework
{
    public abstract class AutofacViewModel<TView> : ViewModel<TView> where TView : IView
    {
        protected AutofacViewModel(TView view, ILifetimeScope scope) : base(view)
        {
            Scope = scope;
        }

        protected ILifetimeScope Scope { get; }

        public override void ActivateInWindow(string state = null)
        {
            var scope = Scope.BeginLifetimeScope();
            var view = scope.Resolve<TView>();
            // ReSharper disable once SuspiciousTypeConversion.Global
            // ReSharper disable once ExpressionIsAlwaysNull
            var screen = view as Screen;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            // ReSharper disable once HeuristicUnreachableCode
            if (screen != null) screen.Deactivated += (sender, args) => {if (args.WasClosed) scope.Dispose();};
            ActivateInWindow(view, state);
        }
    }
}