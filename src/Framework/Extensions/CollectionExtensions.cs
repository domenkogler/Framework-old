using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    public static class CollectionExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in source) action(e, i++);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
        }

        public static void Add<T>(this ICollection<T> collection, params T[] items)
        {
            if (items == null) return;
            collection.Add(items.AsEnumerable());
        }

        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null) return;
            foreach (var i in items)
            {
                collection.Add(i);
            }
        }

        public static void Replace(this IList source, params object[] items)
        {
            source.Clear();
            if (items != null) source.Add(items);
        }

        public static void Replace(this IList source, IEnumerable items)
        {
            source.Clear();
            if (items != null) source.Add(items);
        }

        public static void Replace<T>(this ICollection<T> source, params T[] items)
        {
            source.Clear();
            if (items != null) source.Add(items);
        }

        public static void Replace<T>(this ICollection<T> source, IEnumerable<T> items)
        {
            source.Clear();
            if (items != null) source.Add(items);
        }

        public static int Remove(this IList list, params object[] items)
        {
            object[] remove = list.Cast<object>().Where(items.Contains).ToArray();
            remove.ForEach(list.Remove);
            return remove.Length;
        }

        public static int Remove<T>(this IList<T> list, params T[] items)
        {
            return items.Count(list.Remove);
        }

        public static int Remove(this IList list, Predicate<object> match)
        {
            List<object> matched = list.Cast<object>().Where(item => match(item)).ToList();
            foreach (object item in matched)
            {
                list.Remove(item);
            }
            return matched.Count;
        }
    }
}