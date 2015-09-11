namespace Kogler.Framework
{
    /// <summary>
    /// Interface for the module lifecycle.
    /// </summary>
    public interface IModuleConfiguration
    {
        /// <summary>
        /// Initializes the module controller.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Run the module controller.
        /// </summary>
        void Run();

        /// <summary>
        /// Shutdown the module controller.
        /// </summary>
        void Shutdown();
    }
}