using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public abstract class BootstrapperBase : Caliburn.Micro.BootstrapperBase
    {
        protected BootstrapperBase(bool useApplication) : base(useApplication)
        {
            UseApplication = useApplication;
        }

        protected readonly bool UseApplication;
        protected IEnumerable<IModule> Modules => GetAllInstances(typeof(IModule)).OfType<IModule>();
        protected List<string> AssemblyDirectories { get; } = new List<string>(new[] { "" });
        protected List<string> ModuleNames { get; } = new List<string>();
        private Dictionary<string, Assembly> LoadedAssemblies { get; } = new Dictionary<string, Assembly>();
        
        protected override void PrepareApplication()
        {
            base.PrepareApplication();

            var profileRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.ProductName, "ProfileOptimization");
            Directory.CreateDirectory(profileRoot);
            ProfileOptimization.SetProfileRoot(profileRoot);
            ProfileOptimization.StartProfile("Startup.profile");

            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            var baseAssemblies = new List<Assembly>(base.SelectAssemblies());
            var thisAssembly = GetType().Assembly;
            if (!baseAssemblies.Contains(thisAssembly))
            {
                baseAssemblies.Add(thisAssembly);
            }
            // If this library is being accessed from a Caliburn enabled app then
            // this assembly may already be 'known' in the AssemblySource.Instance collection.
            // We need to remove these otherwise we'll get:
            //  "An item with the same key has already been added." (System.ArgumentException)
            foreach (var assembly in baseAssemblies.ToList().Where(newAssembly => AssemblySource.Instance.Contains(newAssembly)))
            {
                baseAssemblies.Remove(assembly);
            }
            return baseAssemblies;
        }

        protected sealed override void Configure()
        {
            ConfigureLogger();
            LoadAssemblies();
            InitContainer();
            RegisterModules();
            FinishContainer();
            ConfigureLocators();
        }

        protected virtual void ConfigureLogger() { }
        
        protected virtual void LoadAssemblies()
        {
            ModuleNames.Select(a => new KeyValuePair<string, Assembly>(a, Assembly.Load(a))).Apply(kvp =>
            {
                var asi = AssemblySource.Instance;
                // check needed for xUnit tests
                if (!asi.Contains(kvp.Value)) asi.Add(kvp.Value);
                LoadedAssemblies.Add(kvp);
            });
        }

        protected virtual void InitContainer() { }
        
        protected virtual void RegisterModules() { }

        protected virtual void FinishContainer() { }

        protected virtual void InitModules()
        {
            foreach (var module in Modules) { module.Init(); }
            foreach (var module in Modules) { module.InitUI(); }
            foreach (var module in Modules) { module.Run(); }
        }

        protected virtual void ConfigureLocators()
        {
            var baseLocate = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            LocateTypeForModelType(modelType, displayLocation, context) ?? baseLocate(modelType, displayLocation, context);
        }

        public static Func<Type, DependencyObject, object, Type> LocateTypeForModelType = (modelType, displayLocation, context) =>
        {
            Type viewType = modelType.GetGenericSubclassOf(typeof(Screen<>))?.GenericTypeArguments.First();
            if (viewType == null)
            {
                var attribute = modelType.GetCustomAttributes(typeof(ViewAttribute), false)
                        .OfType<ViewAttribute>()
                        .FirstOrDefault(x => x.Context == context);
                viewType = attribute?.ViewType;
            }
            if (viewType != null) return viewType;
            if (viewType != null && viewType.IsInterface)
            {
                return AssemblySource.Instance
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.IsSubclassOf(viewType));
            }
            return null;
        };

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            InitModules();
            DisplayRootView();
        }

        protected virtual void DisplayRootView()
        {
            DisplayRootViewFor<IShellViewModel>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            foreach (var module in Modules.Reverse()) { module.Shutdown(); }
            DestroyContainer();
            base.OnExit(sender, e);
        }

        protected virtual void DestroyContainer() { }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
            HandleException(e.Exception, false);
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) { return; }

            Trace.TraceError(e.ToString());

            if (!isTerminating)
            {
                MessageBox.Show($"Unknown application error\n\n{e}", ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.LeftOf(",").LeftOf(".dll");
            foreach (var path in AssemblyDirectories.Select(dir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir, name + ".dll")))
            {
                if (LoadedAssemblies.ContainsKey(path)) return LoadedAssemblies[path];
                Assembly assembly = null;
                try { assembly = Assembly.LoadFrom(path); }
                catch (FileNotFoundException) { }
                if (assembly == null) continue;
                LoadedAssemblies.Add(path, assembly);
                return assembly;
            }
            return null;
        }
    }
}