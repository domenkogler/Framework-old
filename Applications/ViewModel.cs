using System;

namespace Kogler.Framework
{
    public interface IViewModel
    {
        IView View { get; }
    }

    /// <summary>
    /// Abstract base class for a ViewModel implementation.
    /// </summary>
    public abstract class ViewModel : Model, IViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class and attaches itself as <c>DataContext</c> to the view.
        /// </summary>
        /// <param name="view">The view.</param>
        protected ViewModel(IView view)
        {
            if (view == null) { throw new ArgumentNullException(nameof(view)); }
            View = view;

            // Check if the code is running within the WPF application model
            if (Dispatcher.IsDispatcherSynchronizationContext)
            {
                // Set DataContext of the view has to be delayed so that the ViewModel can initialize the internal data (e.g. Commands)
                // before the view starts with DataBinding.
                Dispatcher.BeginInvoke(() => 
                {
                    View.DataContext = this;
                });
            }
            else
            {
                // When the code runs outside of the WPF application model then we set the DataContext immediately.
                View.DataContext = this;
            }
        }

        /// <summary>
        /// Gets the associated view.
        /// </summary>
        public IView View { get; }
    }

    /// <summary>
    /// Abstract base class for a ViewModel implementation.
    /// </summary>
    /// <typeparam name="TView">The type of the view. Do provide an interface as type and not the concrete type itself.</typeparam>
    public abstract class ViewModel<TView> : ViewModel where TView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel&lt;TView&gt;"/> class and attaches itself as <c>DataContext</c> to the view.
        /// </summary>
        /// <param name="view">The view.</param>
        protected ViewModel(TView view) : base(view)
        {
            ViewCore = view;
        }

        /// <summary>
        /// Gets the associated view as specified view type.
        /// </summary>
        /// <remarks>
        /// Use this property in a ViewModel class to avoid casting.
        /// </remarks>
        protected TView ViewCore { get; }
    }
}