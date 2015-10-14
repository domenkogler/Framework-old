using System;
using System.Linq;
using System.Reflection;

namespace Kogler.Framework
{
    public static class TypeExtensions
    {
        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }

        public static bool HasInterface<TInterface>(this Type type)
        {
            return type.HasInterface(typeof(TInterface));
        }

        public static bool IsGenericSubclassOf(this Type toCheck, Type generic)
        {
            return GetGenericSubclassOf(toCheck, generic) != null;
        }

        public static Type GetGenericSubclassOf(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return toCheck;
                }
                toCheck = toCheck.BaseType;
            }
            return null;
        }

        /// <summary>
		///   Gets the attribute.
		/// </summary>
		/// <param name = "member">The member.</param>
		/// <returns>The member attribute.</returns>
		public static bool HasAttribute<T>(this ICustomAttributeProvider member) where T : class
        {
            return GetAttributes<T>(member).FirstOrDefault() != null;
        }

        /// <summary>
		///   Gets the attribute.
		/// </summary>
		/// <param name = "member">The member.</param>
		/// <returns>The member attribute.</returns>
		public static T GetAttribute<T>(this ICustomAttributeProvider member) where T : class
        {
            return GetAttributes<T>(member).FirstOrDefault();
        }

        /// <summary>
        ///   Gets the attributes. Does not consider inherited attributes!
        /// </summary>
        /// <param name = "member">The member.</param>
        /// <returns>The member attributes.</returns>
        public static T[] GetAttributes<T>(this ICustomAttributeProvider member) where T : class
        {
            if (typeof(T) != typeof(object))
            {
                return (T[])member.GetCustomAttributes(typeof(T), false);
            }
            return (T[])member.GetCustomAttributes(false);
        }
    }
}