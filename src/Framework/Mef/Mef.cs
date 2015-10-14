using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace Kogler.Framework.Mef
{
    public class Mef
    {
        public Mef()
        {
            Catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)));
            Container = new CompositionContainer(Catalog, CompositionOptions.DisableSilentRejection);
        }

        public static Mef Instance { get; } = new Mef();

        public AggregateCatalog Catalog { get; }
        public CompositionContainer Container { get; }
        public CompositionBatch Batch { get; } = new CompositionBatch();

        internal void Compose()
        {
            Batch.AddExportedValue(Container);
            Container.Compose(Batch);
        }

        public void Add(Type type)
        {
            Add(type.Assembly);
        }

        public void Add(Assembly assembly)
        {
            Add(new AssemblyCatalog(assembly));
        }

        public void Add(ComposablePartCatalog catalog)
        {
            var ac = catalog as AssemblyCatalog;
            if (ac != null && Catalog.Catalogs.ContainsFullName(ac)) return;
            Catalog.Catalogs.Add(catalog);
        }
    }
}