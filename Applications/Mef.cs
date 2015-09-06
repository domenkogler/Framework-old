using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Reflection;

namespace Kogler.Framework
{
    public static class Mef
    {
        static Mef()
        {
            Container = new CompositionContainer(Catalog, CompositionOptions.DisableSilentRejection);
        }

        public static AggregateCatalog Catalog { get; } = new AggregateCatalog();
        public static CompositionContainer Container { get; }

        internal static void Compose()
        {
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(Container);
            Container.Compose(batch);
        }

        public static void Add(Type type)
        {
            Add(type.Assembly);
        }

        public static void Add(Assembly assembly)
        {
            Add(new AssemblyCatalog(assembly));    
        }

        public static void Add(ComposablePartCatalog catalog)
        {
            Catalog.Catalogs.Add(catalog);
        }
    }
}