using System;
using System.Collections.Generic;

namespace Kogler.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ViewAttribute : Attribute
    {
        public object Context { get; set; }

        public Type ViewType { get; private set; }

        protected IDictionary<string, object> Metadata { get; }

        public ViewAttribute(IDictionary<string, object> metadata)
        {
            Metadata = metadata;
            ViewType = FromMetadata<Type>("ViewType");
            Context = FromMetadata<object>("Context");
        }

        public ViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }

        protected TType FromMetadata<TType>(string key)
        {
            return Metadata.ContainsKey(key) ? (TType) Metadata[key] : default(TType);
        }
    }
}