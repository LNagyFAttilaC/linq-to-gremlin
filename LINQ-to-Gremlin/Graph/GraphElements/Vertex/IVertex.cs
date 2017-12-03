using System;
using System.Collections.Generic;

namespace LINQtoGremlin.Graph.GraphElements.Vertex
{
    public interface IVertex
    {
        TEntity As<TEntity>()
            where TEntity 
                : IVertex;

        IDictionary<Type, object> AncestorTypeDatabaseIdMappingCache
        {
            get;
        }

        object TopDatabaseId
        {
            get;
        }

        ISet<string> LoadedProperties
        {
            get;
        }
    }
}
