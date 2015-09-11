using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kogler.Framework.Collections;

namespace Kogler.Framework
{
    public static class IEnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return new ObservableCollection<T>(items);
        }

        //public static BulkObservableCollection<T> ToBulkObservableCollection<T>(this IEnumerable<T> items)
        //{
        //    return new BulkObservableCollection<T>(items);
        //}

        public static ReadOnlyObservableCollection<T> ToReadOnlyObservableCollection<T>(this IEnumerable<T> items)
        {
            return new ReadOnlyObservableCollection<T>(items.ToObservableCollection());
        }

        public static TCollection[] ToArrayLock<TCollection>(this IEnumerable<TCollection> collection)
        {
            if (!(collection is ICollection)) return collection.ToArray();
            lock (((ICollection)collection).SyncRoot) { return collection.ToArray(); }
        }
    }
}