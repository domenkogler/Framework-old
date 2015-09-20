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
    public abstract class Bootstrapper : BootstrapperBase
    {
        protected Bootstrapper(bool useApplication) : base(useApplication)
        {
            Initialize();
        }

        protected IEnumerable<IModuleConfiguration> ModuleConfigurations => GetAllInstances(typeof(IModuleConfiguration)).OfType<IModuleConfiguration>();
        protected IEnumerable<IPresentationConfiguration> PresentationConfigurations => GetAllInstances(typeof(IPresentationConfiguration)).OfType<IPresentationConfiguration>();
        protected List<string> AssemblyDirectories { get; } = new List<string>(new[] { "" });
        protected List<string> Modules { get; } = new List<string>();
        protected Dictionary<string, Assembly> LoadedAssemblies { get; } = new Dictionary<string, Assembly>();
        
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

        protected override void Configure()
        {
            LoadAssemblies();
            ConfigureModules();
            InitModules();
            ConfigureLocators();
        }
        
        protected virtual void LoadAssemblies()
        {
            Modules.Select(a => new KeyValuePair<string, Assembly>(a, Assembly.Load(a))).Apply(kvp =>
            {
                var asi = AssemblySource.Instance;
                // check needed for xUnit tests
                if (!asi.Contains(kvp.Value)) asi.Add(kvp.Value);
                LoadedAssemblies.Add(kvp);
            });
        }

        protected virtual void ConfigureModules() { }

        protected virtual void InitModules()
        {
            // Initialize all modules
            foreach (var module in ModuleConfigurations) { module.Initialize(); }
            // Initialize all presentation services
            foreach (var presentation in PresentationConfigurations) { presentation.Initialize(); }
            // Run all modules
            foreach (var module in ModuleConfigurations) { module.Run(); }
        }

        protected virtual void ConfigureLocators()
        {
            var baseLocate = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                Type viewType;
                if (modelType.IsSubclassOf(typeof(Screen<>)))
                {
                    Type screenType;
                    do
                    {
                        screenType = modelType.BaseType;
                    } while (screenType == typeof(Screen<>));
                    viewType = screenType.GenericTypeArguments.First();
                }
                else
                {
                    var attribute =
                        modelType.GetCustomAttributes(typeof(ViewAttribute), false)
                            .OfType<ViewAttribute>()
                            .FirstOrDefault(x => x.Context == context);
                    viewType = attribute?.ViewType;
                }
                if (viewType != null) return viewType;
                if (viewType != null && viewType.IsInterface)
                {
                    return
                        AssemblySource.Instance
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.IsSubclassOf(viewType));
                }
                return baseLocate(modelType, displayLocation, context);
            };
        }
        
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootView();
        }

        protected virtual void DisplayRootView()
        {
            DisplayRootViewFor<IShell>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            foreach (var module in ModuleConfigurations.Reverse()) { module.Shutdown(); }
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