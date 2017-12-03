using LINQtoGremlin.Core.Helper;
using System;
using System.Collections.Generic;

namespace LINQtoGremlin.Graph.GraphElements
{
    public abstract class GraphElement 
        : IGraphElement
    {
        #region Properties

        public Guid Id
        {
            get;
            internal set;
        }
            = Guid
                .NewGuid();

        public object this[
            string propertyName]
        {
            get
            {
                foreach (var property in _extraProperties.Keys)
                {
                    if (property
                        .ToLower() == propertyName
                            .ToLower())
                    {
                        return _extraProperties[property];
                    }
                }

                foreach (var property in GetType()
                    .GetProperties())
                {
                    if (property.Name
                        .ToLower() == propertyName
                            .ToLower())
                    {
                        return GDbReflectionHelper
                            .GetValueOfProperty(
                                this,
                                property.Name);
                    }
                }

                throw new InvalidOperationException(
                    "This object has no '" + propertyName + "' property (case-insensitive).");
            }

            set
            {
                foreach (var property in GetType()
                    .GetProperties())
                {
                    if (property.Name
                            .ToLower() == propertyName
                                .ToLower())
                    {
                        GDbReflectionHelper
                            .SetValueOfProperty(
                                this,
                                property.Name,
                                value);

                        return;
                    }
                }

                _extraProperties[propertyName] = value;
            }
        }

        #endregion

        #region Methods

        public IDictionary<string, object> GetExtraProperties()
            => _extraProperties;

        public bool HasProperty(
            string propertyName)
        {
            foreach (var property in _extraProperties.Keys)
            {
                if (property
                        .ToLower() == propertyName
                            .ToLower())
                {
                    return true;
                }
            }

            foreach (var property in GetType()
                .GetProperties())
            {
                if (property.Name
                        .ToLower() == propertyName
                            .ToLower())
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Fields

        protected readonly IDictionary<string, object> _extraProperties = new Dictionary<string, object>();

        #endregion
    }
}
