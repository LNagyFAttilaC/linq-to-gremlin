using System;

namespace LINQtoGremlin.Core.Helper
{
    public static class GDbReflectionHelper
    {
        #region Methods

        public static Type GetTypeOfProperty(
            Type source,
            string name)
        {
            var propertyInfo
                = source
                    .GetProperty(
                        name);

            return propertyInfo.PropertyType;
        }

        public static Type GetTypeOfProperty(
            object source,
            string name)
            => GetTypeOfProperty(
                source
                    .GetType(),
                name);

        public static object GetValueOfProperty(
            object source,
            string name)
        {
            var propertyInfo
                = source
                    .GetType()
                    .GetProperty(
                        name);

            return propertyInfo
                .GetValue(
                    source);
        }

        public static object SetValueOfProperty(
            object destination,
            string name,
            object value)
        {
            var propertyInfo
                = destination
                    .GetType()
                    .GetProperty(
                        name);

            propertyInfo
                .SetValue(
                    destination,
                    value);

            return destination;
        }

        #endregion
    }
}
