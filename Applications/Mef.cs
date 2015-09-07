using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace Kogler.Framework
{
    public static class Mef
    {
        static Mef()
        {
            Catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            Container = new CompositionContainer(Catalog, CompositionOptions.DisableSilentRejection);
        }

        public static AggregateCatalog Catalog { get; }
        public static CompositionContainer Container { get; }
        public static CompositionBatch Batch { get; } = new CompositionBatch();

        internal static void Compose()
        {
            Batch.AddExportedValue(Container);
            Container.Compose(Batch);
        }

        public static void Add(Type type)
        {
            Add(type.Assembly);
        }

        public static void Add(Assembly assembly)
        {
            Add(new AssemblyCatalog(assembly));  
            AssemblySource.Instance.Add(assembly);  
        }

        public static void Add(ComposablePartCatalog catalog)
        {
            Catalog.Catalogs.Add(catalog);
        }
    }
}