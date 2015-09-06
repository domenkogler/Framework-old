using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kogler.Framework
{
    public static class MemberInfoExtensions
    {
        public static PropertyInfo GetSubProperty(this Type type, string path)
        {
            PropertyInfo pi = null;
            foreach (string propertyName in path.Split('.'))
            {
                pi = type.GetProperty(propertyName);
                type = pi.PropertyType;
            }
            return pi;
        }

        public static object GetSubValue(this Type type, object value, string path, int skip = 0)
        {
            IEnumerable<string> properties = path.Split('.');
            foreach (string propertyName in properties.Reverse().Take(skip).Reverse())
            {
                PropertyInfo pi = type.GetProperty(propertyName);
                value = pi.GetValue(value, null);
                type = pi.PropertyType;
            }
            return value;
        }
    }
}