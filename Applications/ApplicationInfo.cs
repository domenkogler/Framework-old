using System;
using System.IO;
using System.Reflection;

namespace Kogler.Framework
{
    public static class ApplicationInfo
    {
        private static readonly Lazy<Assembly> EntryAssembly = new Lazy<Assembly>(Assembly.GetEntryAssembly);

        private static TAttribute GetAttribute<TAttribute>() where TAttribute : Attribute
        {
            var ea = EntryAssembly.Value;
            if (ea == null) return null;
            return ((TAttribute) Attribute.GetCustomAttribute(ea, typeof (TAttribute)));
        }

        private static readonly Lazy<string> productName = new Lazy<string>(() => GetAttribute<AssemblyProductAttribute>()?.Product);
        private static readonly Lazy<string> description = new Lazy<string>(() => GetAttribute<AssemblyDescriptionAttribute>()?.Description);
        private static readonly Lazy<string> version = new Lazy<string>(() => EntryAssembly.Value?.GetName().Version.ToString());
        private static readonly Lazy<string> company = new Lazy<string>(() => GetAttribute<AssemblyCompanyAttribute>()?.Company);
        private static readonly Lazy<string> copyright = new Lazy<string>(() => GetAttribute<AssemblyCopyrightAttribute>()?.Copyright);
        private static readonly Lazy<string> applicationPath = new Lazy<string>(() => Path.GetDirectoryName(EntryAssembly.Value?.Location ?? ""));
        
        public static string ProductName => productName.Value;
        public static string Description => description.Value;
        public static string Version => version.Value;
        public static string Company => company.Value;
        public static string Copyright => copyright.Value;
        public static string ApplicationPath => applicationPath.Value;
    }
}