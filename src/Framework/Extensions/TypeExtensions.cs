using System;

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
    }
}