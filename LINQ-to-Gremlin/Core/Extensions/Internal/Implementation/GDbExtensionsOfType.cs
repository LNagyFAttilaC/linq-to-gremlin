using System;

namespace LINQtoGremlin.Core.Extensions.Internal
{
    public static class GDbExtensionsOfType
    {
        public static Type GetCollectionType(
            this Type type)
        {
            if (!type.
                IsCollectionType())
            {
                return null;
            }

            return type.GenericTypeArguments[0];
        }

        public static bool HasProperty(
            this Type type,
            string property)
        {
            foreach (var property_ in type
                .GetProperties())
            {
                if (property_.Name == property)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsAnonymousType(
            this Type type)
            => type.Namespace == null && type.Name
                .Contains(
                    "AnonymousType");

        public static bool IsCollectionType(
            this Type type)
        {
            foreach (var interface_ in type
                .GetInterfaces())
            {
                if (interface_.Name
                    .Contains(
                        "IEnumerable"))
                {
                    return type != typeof(string);
                }
            }

            return false;
        }

        public static bool IsSimpleType(
            this Type type)
            => type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal);
    }
}
