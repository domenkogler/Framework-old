using System;
using System.Reflection;

namespace Kogler.Framework
{
    /// <summary>
    /// Provides helper methods to manage resources in WPF.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Gets the pack URI from a local resource path.
        /// </summary>
        /// <param name="resourceAssembly">The assembly containing the resource.</param>
        /// <returns>The pack uri.</returns>
        /// /// <param name="resourcePath">The local resource path.</param>
        public static Uri GetPackUri(this Assembly resourceAssembly, string resourcePath)
        {
            return new Uri("pack://application:,,,/" + resourceAssembly.GetName().Name + ";Component/" + resourcePath);
        }
    }
}