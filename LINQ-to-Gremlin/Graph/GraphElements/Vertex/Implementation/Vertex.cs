using System;
using System.Collections.Generic;

namespace LINQtoGremlin.Graph.GraphElements.Vertex
{
    public class Vertex 
        : GraphElement
        , IVertex
    {
        #region Properties

        public IDictionary<Type, object> AncestorTypeDatabaseIdMappingCache
            => _ancestorTypeDatabaseIdMappingCache;

        public ISet<string> LoadedProperties
            => _loadedProperties;

        public object TopDatabaseId
            => AncestorTypeDatabaseIdMappingCache
                .ContainsKey(
                    GetType())
                ? AncestorTypeDatabaseIdMappingCache[GetType()]
                : null;

        #endregion

        #region Methods

        public TEntity As<TEntity>()
            where TEntity
                : IVertex
        {
            if (!(this is TEntity))
            {
                throw new InvalidCastException(
                    "Can not cast '" + GetType().Name + "' to '" + typeof(TEntity).Name + "'.");
            }

            return (TEntity)
                Convert
                    .ChangeType(
                        this,
                        typeof(TEntity));
        }

        #endregion

        #region Fields

        protected readonly IDictionary<Type, object> _ancestorTypeDatabaseIdMappingCache = new Dictionary<Type, object>();

        protected readonly ISet<string> _loadedProperties = new HashSet<string>();

        #endregion
    }
}
