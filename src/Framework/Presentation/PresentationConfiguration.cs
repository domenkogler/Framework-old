using System.Reflection;
using System.Windows;

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

    public abstract class PresentationConfiguration : IPresentationConfiguration
    {
        public void AddAppDictionaries(params string[] moduleResources)
        {
            AddAppDictionaries(GetType().Assembly, moduleResources);
        }

        public void AddAppDictionaries(Assembly resourceAssembly, params string[] moduleResources)
        {
            var mergedDictionaries = Application.Current?.Resources.MergedDictionaries;

            foreach (var resourcePath in moduleResources)
            {
                mergedDictionaries?.Add(new ResourceDictionary { Source = resourceAssembly.GetPackUri(resourcePath) });
            }
        }

        void IPresentationConfiguration.Initialize()
        {
            Dispatcher.OnUIThread(Initialize);
        }

        public abstract void Initialize();
    }
}