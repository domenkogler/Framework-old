namespace Kogler.Framework
{
    public abstract class MefViewModel<TView> : ViewModel<TView> where TView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel&lt;TView&gt;"/> class and attaches itself as <c>DataContext</c> to the view.
        /// </summary>
        /// <param name="exportFactory">The view.</param>
        protected MefViewModel(MefExportFactory<TView> exportFactory) : base(exportFactory.Instance)
        {
            ExportFactory = exportFactory;
        }

        protected MefExportFactory<TView> ExportFactory { get; }

        public override void ActivateInWindow(string state = null)
        {
            ActivateInWindow(ExportFactory.Create(), state);
        }
    }
}