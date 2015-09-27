using System.Reflection;
using System.Windows;

namespace Kogler.Framework
{
    /// <summary>
    /// Interface for the module lifecycle.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        void Load();

        /// <summary>
        /// Initializes the UI.
        /// </summary>
        void InitUI();

        /// <summary>
        /// Run the module.
        /// </summary>
        void Run();

        /// <summary>
        /// Shutdown (cleanup) the module.
        /// </summary>
        void Shutdown();
    }

    public abstract class Module : IModule
    {
        public static void AddAppDictionaries(Assembly resourceAssembly, params string[] moduleResources)
        {
            var mergedDictionaries = Application.Current?.Resources.MergedDictionaries;

            foreach (var resourcePath in moduleResources)
            {
                mergedDictionaries?.Add(new ResourceDictionary { Source = resourceAssembly.GetPackUri(resourcePath) });
            }
        }

        public void AddAppDictionaries(params string[] resources)
        {
            AddAppDictionaries(GetType().Assembly, resources);
        }

        void IModule.InitUI()
        {
            Dispatcher.OnUIThread(InitUI);
        }

        public virtual void Load() { }

        public virtual void InitUI() { }

        public virtual void Run() { }

        public virtual void Shutdown() { }
    }
}