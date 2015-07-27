using System;

namespace Kogler.Framework
{
    /// <summary>
    /// Abstract base class for a ViewModel implementation.
    /// </summary>
    public abstract class ViewModel : Model
    {
        private readonly IView view;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class and attaches itself as <c>DataContext</c> to the view.
        /// </summary>
        /// <param name="view">The view.</param>
        protected ViewModel(IView view)
        {
            if (view == null) { throw new ArgumentNullException(nameof(view)); }
            this.view = view;

            // Check if the code is running within the WPF application model
            if (Dispatcher.IsDispatcherSynchronizationContext)
            {
                // Set DataContext of the view has to be delayed so that the ViewModel can initialize the internal data (e.g. Commands)
                // before the view starts with DataBinding.
                Dispatcher.BeginInvoke(() => 
                {
                    this.view.DataContext = this;
                });
            }
            else
            {
                // When the code runs outside of the WPF application model then we set the DataContext immediately.
                view.DataContext = this;
            }
        }

        /// <summary>
        /// Gets the associated view.
        /// </summary>
        public object View => view;
    }
}