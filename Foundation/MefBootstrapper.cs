using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
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
    public class MefBootstrapper : BootstrapperBase
    {
        public MefBootstrapper() : base()
        {
            Initialize();
        }

        protected IEnumerable<IModuleController> ModuleControllers => Mef.Container.GetExportedValues<IModuleController>();
        protected IEnumerable<IPresentationService> PresentationServices => Mef.Container.GetExportedValues<IPresentationService>();
        protected List<string> AssemblyDirectories { get; } = new List<string>(new[] { "" });
        protected List<string> MefAssemblies { get; } = new List<string>();
        private readonly List<string> resolvedAssemblies = new List<string>();

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

        protected virtual void PrepareAssemblies()
        {
            
        }

        protected override void Configure()
        {
            Mef.Add(typeof(App));
            Mef.Add(GetType());
            PrepareAssemblies();
            MefAssemblies.Select(Assembly.Load).Apply(Mef.Add);

            Mef.Batch.AddExportedValue<IWindowManager>(new WindowManager());
            Mef.Batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            Mef.Compose();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = Mef.Container.GetExportedValues<object>(contract);

            if (exports.Any()) return exports.First();

            throw new Exception($"Could not locate any instances of contract {contract}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Mef.Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            Mef.Container.SatisfyImportsOnce(instance);
        }

        protected virtual void ConfigureLocators()
        {
            var baseLocate = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (modelType, displayLocation, context) =>
            {
                Type viewType;
                if (modelType.IsSubclassOf(typeof (Screen<>)))
                {
                    Type screenType;
                    do
                    {
                        screenType = modelType.BaseType;
                    } while (screenType == typeof (Screen<>));
                    viewType = screenType.GenericTypeArguments.First();
                }
                else
                {
                    var attribute =
                        modelType.GetCustomAttributes(typeof (ViewAttribute), false)
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
            ConfigureLocators();

            // Initialize all presentation services
            foreach (var presentationService in PresentationServices) { presentationService.Initialize(); }

            // Initialize and run all module controllers
            foreach (var moduleController in ModuleControllers) { moduleController.Initialize(); }
            foreach (var moduleController in ModuleControllers) { moduleController.Run(); }
            

            DisplayRootView();
        }

        protected virtual void DisplayRootView()
        {
            var wm = Mef.Container.GetExportedValue<IWindowManager>();
            DisplayRootViewFor<IShell>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            foreach (var moduleController in ModuleControllers.Reverse()) { moduleController.Shutdown(); }
            Mef.Container.Dispose();
            Mef.Catalog.Dispose();
            base.OnExit(sender, e);
        }

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
                if (resolvedAssemblies.Contains(path)) return null;
                Assembly assembly = null;
                try { assembly = Assembly.LoadFrom(path); }
                catch (FileNotFoundException) { }
                if (assembly == null) continue;
                resolvedAssemblies.Add(path);
                return assembly;
            }
            return null;
        }
    }
}