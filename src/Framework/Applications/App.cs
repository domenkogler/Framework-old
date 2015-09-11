using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Windows;
using System.Windows.Threading;

namespace Kogler.Framework
{
    public class App : Application
    {
        protected IEnumerable<IModuleController> ModuleControllers => Mef.Container.GetExportedValues<IModuleController>();
        protected IEnumerable<IPresentationService> PresentationServices => Mef.Container.GetExportedValues<IPresentationService>();
        protected List<string> AssemblyDirectories { get; } = new List<string>(new[] {""});
        protected List<string> MefAssemblies { get; } = new List<string>();
        private readonly List<string> ResolvedAssemblies = new List<string>(); 

        public App()
        {
            var profileRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationInfo.ProductName, "ProfileOptimization");
            Directory.CreateDirectory(profileRoot);
            ProfileOptimization.SetProfileRoot(profileRoot);
            ProfileOptimization.StartProfile("Startup.profile");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AssemblyResolve;

            Mef.Add(typeof(App));
            Mef.Add(GetType());
            foreach (var assembly in MefAssemblies)
            {
                Mef.Add(Assembly.Load(assembly));
            }
            Mef.Compose();

            // Initialize all presentation services
            foreach (var presentationService in PresentationServices) { presentationService.Initialize(); }

            // Initialize and run all module controllers
            foreach (var moduleController in ModuleControllers) { moduleController.Initialize(); }
            foreach (var moduleController in ModuleControllers) { moduleController.Run(); }
        }
        
        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.LeftOf(",").LeftOf(".dll");
            foreach (var path in AssemblyDirectories.Select(dir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dir, name + ".dll")))
            {
                if (ResolvedAssemblies.Contains(path)) return null;
                Assembly assembly = null;
                try { assembly = Assembly.LoadFrom(path); }
                catch (FileNotFoundException) { }
                if (assembly == null) continue;
                ResolvedAssemblies.Add(path);
                return assembly;
            }
            return null;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (var moduleController in ModuleControllers.Reverse()) { moduleController.Shutdown(); }
            Mef.Container.Dispose();
            Mef.Catalog.Dispose();
            base.OnExit(e);
        }

        private static void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
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
    }
}