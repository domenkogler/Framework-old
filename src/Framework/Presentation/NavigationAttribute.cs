using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace Kogler.Framework
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class NavigationAttribute : ViewAttribute, IHaveDisplayName
    {
        public string DisplayName { get; set; }
        public string Path { get; private set; }

        public NavigationAttribute(IDictionary<string, object> metadata) : base(metadata)
        {
            Path = FromMetadata<string>("Path");
            DisplayName = FromMetadata<string>("DisplayName");
        }

        public NavigationAttribute(Type viewType, string path) : base (viewType)
        {
            Path = path;
        }

        public static string GetDisplayName<TType>(TType type, NavigationAttribute attribute, string fallback = null)
        {
            if (!string.IsNullOrEmpty(attribute.DisplayName)) return attribute.DisplayName;
            return type.GetType().HasInterface<IHaveDisplayName>() ? ((IHaveDisplayName) type).DisplayName : fallback;
        }
    }
}