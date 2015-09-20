using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;

namespace Kogler.Framework
{
    public static class AssemblyExtensions
    {
        public static bool ContainsFullName(this IEnumerable<Assembly> collection, Assembly assembly)
        {
            return collection.Any(a => a.FullName == assembly.FullName);
        }

        public static bool ContainsFullName(this IEnumerable<ComposablePartCatalog> collection, AssemblyCatalog assembly)
        {
            return collection.OfType<AssemblyCatalog>().Select(c => c.Assembly).Contains(assembly.Assembly);
        }
    }
}