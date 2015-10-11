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
        /// Init the module.
        /// </summary>
        void Init();

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
        public static void AddToAppMergedDictionaries(Assembly resourceAssembly, params string[] paths)
        {
            var mergedDictionaries = Application.Current?.Resources.MergedDictionaries;

            foreach (var resourcePath in paths)
            {
                mergedDictionaries?.Add(new ResourceDictionary { Source = resourceAssembly.GetPackUri(resourcePath) });
            }
        }

        public void AddToAppMergedDictionaries(params string[] paths)
        {
            AddToAppMergedDictionaries(GetType().Assembly, paths);
        }

        public virtual void Init() { }

        public virtual void InitUI() { }

        void IModule.InitUI()
        {
            Dispatcher.OnUIThread(InitUI);
        }

        public virtual void Run() { }
        
        public virtual void Shutdown() { }
    }
}