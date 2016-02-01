using System;
using System.Collections.Generic;
using System.Linq;

namespace Kogler.Framework
{
    public static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> source, IDictionary<TKey, TValue> dictionary, Func<IGrouping<TKey, KeyValuePair<TKey, TValue>>, TValue> selector = null)
        {
            if (dictionary == null) return source;
            if (selector == null) selector = d => d.Last().Value;
            return source.Concat(dictionary).GroupBy(d => d.Key).ToDictionary(d => d.Key, selector);
        }
    }
}
