using System;
using System.Collections.Generic;

namespace Kogler.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ViewAttribute : Attribute
    {
        public object Context { get; set; }

        public Type ViewType { get; private set; }

        public ViewAttribute(IDictionary<string, object> metadata)
        {
            ViewType = (Type)metadata["ViewType"];
            Context = metadata["Context"];
        }

        public ViewAttribute(Type viewType)
        {
            ViewType = viewType;
        }
    }
}