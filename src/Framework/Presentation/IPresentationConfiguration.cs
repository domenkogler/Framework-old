namespace Kogler.Framework
{
    /// <summary>
    /// Service for initializing the presentation layer. These services are called before any ModuleController is Runed
    /// and after all ModuleConfiguration ara Initialized.
    /// </summary>
    /// <remarks>
    /// This service can be used to initialize the culture settings (must be done before the first view is created) 
    /// or to register resource dictionaries.
    /// </remarks>
    public interface IPresentationConfiguration
    {
        /// <summary>
        /// Initializes the service.
        /// </summary>
        void Initialize();
    }
}