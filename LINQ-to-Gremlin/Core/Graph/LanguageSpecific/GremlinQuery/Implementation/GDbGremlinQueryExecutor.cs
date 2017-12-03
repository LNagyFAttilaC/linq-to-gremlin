using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using LINQtoGremlin.Core.Diagnostics.Exceptions;
using LINQtoGremlin.Core.Extensions.Internal;
using LINQtoGremlin.Core.Helper;
using LINQtoGremlin.Core.Infrastructure.Internal;
using LINQtoGremlin.Core.Query.IntermediateTree;
using LINQtoGremlin.Graph.GraphElements.Vertex;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Graph.LanguageSpecific.GremlinQuery
{
    public class GDbGremlinQueryExecutor 
        : IGDbQueryExecutor
        , IGDbGremlinQueryExecutor
    {
        #region Constructors

        public GDbGremlinQueryExecutor(
            IDbContextOptions dbContextOptions)
        {
            ContextOptionsExtension
                = dbContextOptions.Extensions
                    .OfType<GDbContextOptionsExtension>()
                    .First();

            g
                = _graph
                    .Traversal()
                    .WithRemote(
                        new DriverRemoteConnection(
                            new GremlinClient(
                                new GremlinServer(
                                    ContextOptionsExtension.DatabaseCredentials.Host,
                                    ContextOptionsExtension.DatabaseCredentials.Port,
                                    ContextOptionsExtension.DatabaseCredentials.EnableSSL,
                                    ContextOptionsExtension.DatabaseCredentials.Username,
                                    ContextOptionsExtension.DatabaseCredentials.Password))));
        }

        #endregion

        #region Properties

        private GDbContextOptionsExtension ContextOptionsExtension
        {
            get;
        }

        #endregion

        #region Methods

        public int Create(
            IEnumerable<IUpdateEntry> updateEntries)
        {
            var createdVertices = 0;

            /*
             * ancestors
             */

            IDictionary<Type, IEnumerable<Type>> ancestorsCache =
                CreateAncestorCache(
                    updateEntries);

            /*
             * ancestor vertices, database ids
             */
            
            foreach (var updateEntry in updateEntries)
            {
                foreach (var ancestor in ancestorsCache[updateEntry.EntityType.ClrType]
                    .Reverse())
                {
                    // adding vertex for ancestor

                    var query_addVertexWithLabel
                        = GDbGremlinQueries
                            .GetAddVertexWithLabelQuery(
                                g,
                                GetLabelOfType(
                                    ancestor))
                            .Id();

                    ((Vertex)
                        (updateEntry
                            .ToEntityEntry().Entity))
                        .AncestorTypeDatabaseIdMappingCache[ancestor] 
                            = long
                                .Parse(
                                    query_addVertexWithLabel
                                        .Next()
                                        .ToString());
                }

                createdVertices++;
            }

            foreach (var updateEntry in updateEntries)
            {
                var entry
                    = (Vertex)
                        (updateEntry
                            .ToEntityEntry().Entity);

                IDictionary<IGDbModelDescriptorEntry, object> ancestorProperties = new Dictionary<IGDbModelDescriptorEntry, object>();
                IDictionary<IGDbModelDescriptorEntry, IList<object>> ancestorEdges = new Dictionary<IGDbModelDescriptorEntry, IList<object>>();

                foreach (var ancestor in ancestorsCache[updateEntry.EntityType.ClrType]
                    .Reverse())
                {
                    /*
                     * ancestor properties
                     */
                    
                    foreach (var ancestorProperty in ancestorProperties)
                    {
                        // connecting vertex with ancestor property

                        var query_addEdgeWithLabelFromVertexByIdToVertexById
                            = GDbGremlinQueries
                                .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                    g,
                                    entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                    ancestorProperty.Value,
                                    ancestorProperty.Key.EntryName);

                        query_addEdgeWithLabelFromVertexByIdToVertexById
                            .Next();
                    }

                    /*
                     * properties
                     */

                    var properties
                        = ContextOptionsExtension
                            .GetModelDescriptor(
                                ancestor)
                            .GetModelDescriptorEntriesByType(
                                GDbModelDescriptorEntryTypes.PROPERTY);

                    foreach (var property in properties)
                    {
                        ancestorProperties[property]
                            = AddPropertyToVertex(
                                entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                property.EntryName,
                                GDbReflectionHelper
                                    .GetValueOfProperty(
                                        entry,
                                        property.EntryValue
                                            .ToString()));
                    }

                    /*
                     * extra properties
                     */
                    
                    if (ancestor == updateEntry.EntityType.ClrType)
                    {
                        var extraProperties
                            = entry
                                .GetExtraProperties();

                        foreach (var extraProperty in extraProperties)
                        {
                            AddPropertyToVertex(
                                entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                extraProperty.Key,
                                extraProperty.Value);
                        }
                    }

                    /*
                     * ancestor edges
                     */
                    
                    foreach (var ancestorEdge in ancestorEdges)
                    {
                        foreach (var otherEntryId in ancestorEdge.Value)
                        {
                            var query_addEdgeWithLabelFromVertexByIdToVertexById
                                = GDbGremlinQueries
                                    .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                        g,
                                        entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                        otherEntryId,
                                        ancestorEdge.Key.EntryName);

                            query_addEdgeWithLabelFromVertexByIdToVertexById
                                .Next();
                        }
                    }

                    /*
                     * edges
                     */
                    
                    var edges
                        = ContextOptionsExtension
                            .GetModelDescriptor(
                                ancestor)
                            .GetModelDescriptorEntriesByType(
                                GDbModelDescriptorEntryTypes.EDGE);

                    foreach (var edge in edges)
                    {
                        // after syncing, edge can and should be handled as loaded

                        entry.LoadedProperties
                            .Add(
                                edge.EntryValue
                                    .ToString());

                        var isMultipleEdge
                            = GDbReflectionHelper
                                .GetTypeOfProperty(
                                    entry,
                                    edge.EntryValue
                                        .ToString())
                                .IsCollectionType();

                        if (!ancestorEdges
                            .ContainsKey(
                                edge))
                        {
                            ancestorEdges[edge] = new List<object>();
                        }

                        if (isMultipleEdge)
                        {
                            var otherEntries
                                = (IEnumerable)
                                    GDbReflectionHelper
                                        .GetValueOfProperty(
                                            entry,
                                            edge.EntryValue
                                                .ToString());

                            if (otherEntries != null)
                            {
                                foreach (var otherEntry in otherEntries)
                                {
                                    if (otherEntry != null)
                                    {
                                        foreach (var ancestorOfOtherEntry in ((Vertex)
                                            otherEntry).AncestorTypeDatabaseIdMappingCache.Values)
                                        {
                                            var query_addEdgeWithLabelFromVertexByIdToVertexById
                                                = GDbGremlinQueries
                                                    .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                                        g,
                                                        entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                                        ancestorOfOtherEntry,
                                                        edge.EntryName);

                                            query_addEdgeWithLabelFromVertexByIdToVertexById
                                                .Next();

                                            ancestorEdges[edge]
                                                .Add(
                                                    ancestorOfOtherEntry);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var otherEntry
                                = (Vertex)
                                    GDbReflectionHelper
                                        .GetValueOfProperty(
                                            entry,
                                            edge.EntryValue
                                                .ToString());

                            if (otherEntry != null)
                            {
                                foreach (var ancestorOfOtherEntry in otherEntry.AncestorTypeDatabaseIdMappingCache.Values)
                                {
                                    var query_addEdgeWithLabelFromVertexByIdToVertexById
                                        = GDbGremlinQueries
                                            .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                                g,
                                                entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                                ancestorOfOtherEntry,
                                                edge.EntryName);

                                    query_addEdgeWithLabelFromVertexByIdToVertexById
                                        .Next();

                                    ancestorEdges[edge]
                                        .Add(
                                            ancestorOfOtherEntry);
                                }
                            }
                        }
                    }
                }
            }

            return createdVertices;
        }

        public Task<int> CreateAsync(
            IEnumerable<IUpdateEntry> updateEntries,
            CancellationToken cancellationToken = default(CancellationToken))
            => Task
                .FromResult(
                    Create(
                        updateEntries));

        public int Delete(
            IEnumerable<IUpdateEntry> updateEntries)
        {
            var deletedVertices = 0;

            foreach (var updateEntry in updateEntries)
            {
                var entry
                    = (Vertex)
                        (updateEntry
                            .ToEntityEntry().Entity);

                /*
                 * ancestors
                 */

                var query
                    = g
                        .V(
                            entry.TopDatabaseId)
                        .Out(
                            GDbModelConstants._INNER_APPLICATION_ID_NAME)
                        .In();

                var ancestorIds
                    = query
                        .Id()
                        .ToList();

                foreach (var ancestorId in ancestorIds)
                {
                    /*
                     * ancestor properties
                     */

                    query
                        = g
                            .V(
                                ancestorId)
                            .Out()
                            .HasLabel(
                                GDbModelConstants._INNER_PROPERTY_VERTEX_NAME);

                    query
                        .Drop()
                        .Next();

                    /*
                     * ancestor vertices (including its edges)
                     */

                    query
                        = GDbGremlinQueries
                            .GetDropVertexByIdQuery(
                                g,
                                ancestorId);
                    query
                        .Next();
                }

                entry.AncestorTypeDatabaseIdMappingCache
                    .Clear();

                entry.LoadedProperties
                    .Clear();

                deletedVertices++;
            }

            return deletedVertices;
        }

        public Task<int> DeleteAsync(
            IEnumerable<IUpdateEntry> updateEntries, 
            CancellationToken cancellationToken = default(CancellationToken))
            => Task
                .FromResult(
                    Delete(
                        updateEntries));

        public IEnumerable<TEntity> Read<TEntity>(
            Type rootType,
            IEnumerable<IGDbQueryOperation> intermediateTree)
        {
            var rootModelDescriptor
                = ContextOptionsExtension
                    .GetModelDescriptor(
                        rootType);

            if (rootModelDescriptor == null)
            {
                throw new InvalidOperationException(
                    "Invalid query. Root type '" + rootType.Name + "' has no model descriptor.");
            }

            /*
             * wheres
             */

            var whereOperations
                = intermediateTree
                    .Where(
                        o => o
                            .GetName() == GDbQueryOperationNames.WHERE);

            if (whereOperations
                .Count() > 1)
            {
                throw new InvalidOperationException(
                    "There are more than one 'WHERE' clauses. Maximum one is allowed.");
            }

            /*
             * orderings
             */

            GDbQueryOperationOrderBy gDbQueryOperationOrderBy = null;
            IList<GDbQueryOperationThenBy> gDbQueryOperationThenBys = new List<GDbQueryOperationThenBy>();

            foreach (var orderingOperation in intermediateTree
                .Where(
                    o => o
                        .GetName() == GDbQueryOperationNames.ORDER_BY || o
                            .GetName() == GDbQueryOperationNames.THEN_BY)
                .Reverse())
            {
                if (orderingOperation is GDbQueryOperationOrderBy)
                {
                    gDbQueryOperationOrderBy
                        = (GDbQueryOperationOrderBy)
                            orderingOperation;

                    break;
                }

                if (orderingOperation is GDbQueryOperationThenBy)
                {
                    gDbQueryOperationThenBys
                        .Add(
                            (GDbQueryOperationThenBy)
                                orderingOperation);
                }
            }

            /*
             * selects
             */

            var selectOperations
                = intermediateTree
                    .Where(
                        o => o
                            .GetName() == GDbQueryOperationNames.SELECT);

            if (selectOperations
                .Count() > 1)
            {
                throw new InvalidOperationException(
                    "There are more than one 'SELECT' clauses. Maximum one is allowed.");
            }

            var selectedProperties
                = selectOperations
                    .Count() > 0
                        ? selectOperations
                            .First()
                            .GetArguments()
                            .Cast<GDbQueryOperationPropertySelector>()
                        : null;

            /*
             * result operators
             */

            var resultOperations
                = intermediateTree
                    .Where(
                        o => o
                            .GetName() == GDbQueryOperationNames.ANY || o
                                .GetName() == GDbQueryOperationNames.AVERAGE || o
                                    .GetName() == GDbQueryOperationNames.COUNT || o
                                        .GetName() == GDbQueryOperationNames.FIRST || o
                                            .GetName() == GDbQueryOperationNames.LAST || o
                                                .GetName() == GDbQueryOperationNames.LONG_COUNT || o
                                                    .GetName() == GDbQueryOperationNames.MAX || o
                                                        .GetName() == GDbQueryOperationNames.MIN || o
                                                            .GetName() == GDbQueryOperationNames.SINGLE || o
                                                                .GetName() == GDbQueryOperationNames.SKIP || o
                                                                    .GetName() == GDbQueryOperationNames.SUM || o
                                                                        .GetName() == GDbQueryOperationNames.TAKE);

            /*
             * includes
             */

            var includeChains
                = intermediateTree
                    .Where(
                        o => o
                            .GetName() == GDbQueryOperationNames.INCLUDE_CHAIN)
                    .Cast<GDbQueryOperationIncludeChain>();

            /*
             * main query
             */

            var query
                = BuildQueryPartOrdering(
                    rootType,
                    BuildQueryPartWhere(
                        rootType,
                        GDbGremlinQueries
                            .GetGetVerticesByLabelQuery(
                                g,
                                GetLabelOfType(
                                    rootType)),
                        whereOperations
                            .Count() > 0
                                ? (GDbQueryOperationWhere)
                                    whereOperations
                                        .First()
                                : null),
                    gDbQueryOperationOrderBy,
                    gDbQueryOperationThenBys);

            /*
             * final query
             */

            var defaultMaterializationNeeded = false;

            var theRestResultOperatorsMatter = true;

            var requiredCardinality = -1;

            GraphTraversal<Gremlin.Net.Structure.Vertex, object> finalQuery = null;

            foreach (var resultOperation in resultOperations)
            {
                switch (resultOperation
                    .GetName())
                {
                    case GDbQueryOperationNames.ANY:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            query
                                                .Id()
                                                .Count()
                                                .Next() > 0,
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.AVERAGE:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            BuildQueryPartSelect(
                                                query,
                                                selectedProperties)
                                            .Mean<double>()
                                            .Next(),
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.COUNT:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            query
                                                .Id()
                                                .Count()
                                                .Next(),
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.FIRST:
                        finalQuery
                            = query
                                .Limit<object>(
                                    1);

                        defaultMaterializationNeeded
                            = (bool)
                                RetrieveConstantValueFromGDbQueryOperationConstant(
                                    (GDbQueryOperationConstant)
                                        resultOperation
                                            .GetArguments()
                                            .ElementAt(
                                                0));

                        theRestResultOperatorsMatter = false;
                        break;
                    case GDbQueryOperationNames.LAST:
                        finalQuery
                            = query
                                .Tail<object>(
                                    1);

                        defaultMaterializationNeeded
                            = (bool)
                                RetrieveConstantValueFromGDbQueryOperationConstant(
                                    (GDbQueryOperationConstant)
                                        resultOperation
                                            .GetArguments()
                                            .ElementAt(
                                                0));

                        theRestResultOperatorsMatter = false;
                        break;
                    case GDbQueryOperationNames.LONG_COUNT:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            query
                                                .Id()
                                                .Count()
                                                .Next(),
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.MAX:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            BuildQueryPartSelect(
                                                query,
                                                selectedProperties)
                                            .Max<TEntity>()
                                            .Next(),
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.MIN:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            BuildQueryPartSelect(
                                                query,
                                                selectedProperties)
                                            .Min<TEntity>()
                                            .Next(),
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.SINGLE:
                        finalQuery
                            = query
                                .Limit<object>(
                                    1);

                        defaultMaterializationNeeded
                            = (bool)
                                RetrieveConstantValueFromGDbQueryOperationConstant(
                                    (GDbQueryOperationConstant)
                                        resultOperation
                                            .GetArguments()
                                            .ElementAt(
                                                0));

                        theRestResultOperatorsMatter = false;
                        break;
                    case GDbQueryOperationNames.SKIP:
                        finalQuery
                            = query
                                .Skip<object>(
                                    RetrieveConstantValueFromGDbQueryOperationConstant(
                                        (GDbQueryOperationConstant)
                                            resultOperation
                                                .GetArguments()
                                                .ElementAt(
                                                    0)));

                        theRestResultOperatorsMatter = true;
                        break;
                    case GDbQueryOperationNames.SUM:
                        return new List<TEntity>()
                            {
                                (TEntity)
                                    Convert
                                        .ChangeType(
                                            BuildQueryPartSelect(
                                                query,
                                                selectedProperties)
                                            .Sum<TEntity>()
                                            .Next(),
                                            typeof(TEntity))
                            };
                    case GDbQueryOperationNames.TAKE:
                        finalQuery
                            = query
                                .Limit<object>(
                                    RetrieveConstantValueFromGDbQueryOperationConstant(
                                        (GDbQueryOperationConstant)
                                            resultOperation
                                                .GetArguments()
                                                .ElementAt(
                                                    0)));

                        theRestResultOperatorsMatter = true;
                        break;
                }

                if (!theRestResultOperatorsMatter)
                {
                    break;
                }
            }

            /*
             * materializing results
             */

            IList<TEntity> resultEntities = new List<TEntity>();

            if (!defaultMaterializationNeeded)
            {
                /*
                 * wanted ids
                 */

                var resultIds
                    = finalQuery != null
                        ? query
                            .Id()
                            .ToList()
                        : query
                            .Id()
                            .ToList();

                if (requiredCardinality >= 0 && resultIds.Count != requiredCardinality)
                {
                    throw new GDbDatabaseIntegrityDamagedException(
                        "Query has more or less than " + requiredCardinality + " result(s).");
                }
                
                foreach (var resultId in resultIds)
                {
                    resultEntities
                        .Add(
                            MaterializeEntity<TEntity>(
                                rootType,
                                resultId,
                                selectedProperties,
                                includeChains));
                }
            }
            else
            {
                resultEntities
                    .Add(
                        default(TEntity));
            }

            return resultEntities;
        }

        public Task<IEnumerable<TEntity>> ReadAsync<TEntity>(
            Type rootType,
            IEnumerable<IGDbQueryOperation> intermediateTree,
            CancellationToken cancellationToken = default(CancellationToken))
            => Task
                .FromResult(
                    Read<TEntity>(
                        rootType,
                        intermediateTree));

        public int Update(
            IEnumerable<IUpdateEntry> updateEntries)
        {
            var updatedVertices = 0;

            /*
             * ancestors
             */

            IDictionary<Type, IEnumerable<Type>> ancestorsCache =
                CreateAncestorCache(
                    updateEntries);

            foreach (var updateEntry in updateEntries)
            {
                var entry
                    = (Vertex)
                        (updateEntry
                            .ToEntityEntry().Entity);

                IDictionary<IGDbModelDescriptorEntry, IList<object>> ancestorEdgesToCreate = new Dictionary<IGDbModelDescriptorEntry, IList<object>>();
                IDictionary<IGDbModelDescriptorEntry, IList<object>> ancestorEdgesToDelete = new Dictionary<IGDbModelDescriptorEntry, IList<object>>();

                foreach (var ancestor in ancestorsCache[updateEntry.EntityType.ClrType]
                    .Reverse())
                {
                    /*
                     * properties
                     */

                    var properties
                        = ContextOptionsExtension
                            .GetModelDescriptor(
                                ancestor)
                            .GetModelDescriptorEntriesByType(
                                GDbModelDescriptorEntryTypes.PROPERTY);

                    foreach (var property in properties)
                    {
                        AddOrUpdatePropertyToOrOfVertex(
                            entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                            property.EntryName,
                            GDbReflectionHelper
                                .GetValueOfProperty(
                                    entry,
                                    property.EntryValue
                                        .ToString()));
                    }

                    /*
                     * extra properties
                     */

                    if (ancestor == updateEntry.EntityType.ClrType)
                    {
                        var extraProperties
                            = entry
                                .GetExtraProperties();

                        foreach (var extraProperty in extraProperties)
                        {
                            AddOrUpdatePropertyToOrOfVertex(
                                entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                extraProperty.Key,
                                extraProperty.Value);
                        }
                    }

                    /*
                     * ancestor edges to add
                     */
                    
                    foreach (var ancestorEdge in ancestorEdgesToCreate)
                    {
                        foreach (var otherEntryId in ancestorEdge.Value)
                        {
                            var query_addEdgeWithLabelFromVertexByIdToVertexById
                                = GDbGremlinQueries
                                    .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                        g,
                                        entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                        otherEntryId,
                                        ancestorEdge.Key.EntryName);

                            query_addEdgeWithLabelFromVertexByIdToVertexById
                                .Next();
                        }
                    }

                    /*
                     * ancestor edges to delete
                     */

                    foreach (var ancestorEdge in ancestorEdgesToDelete)
                    {
                        var query_getOutEdgesByLabelOfVertexById
                            = GDbGremlinQueries
                                .GetGetOutEdgesByLabelOfVertexByIdQuery(
                                    g,
                                    entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                    ancestorEdge.Key.EntryName);

                        var ownEdges
                            = query_getOutEdgesByLabelOfVertexById
                                .ToList();

                        foreach (var otherEntryId in ancestorEdge.Value)
                        {
                            foreach (var ownEdge in ownEdges)
                            {
                                if (ownEdge.InV.Id == otherEntryId)
                                {
                                    var query_dropEdgeById
                                        = GDbGremlinQueries
                                            .GetDropEdgeByIdQuery(
                                                g,
                                                ownEdge.Id);

                                    query_dropEdgeById
                                        .Next();
                               }
                            }
                        }
                    }

                    /*
                     * edges
                     */

                    var edges
                        = ContextOptionsExtension
                            .GetModelDescriptor(
                                ancestor)
                            .GetModelDescriptorEntriesByType(
                                GDbModelDescriptorEntryTypes.EDGE);

                    foreach (var edge in edges)
                    {
                        var isMultipleEdge
                            = GDbReflectionHelper
                                .GetTypeOfProperty(
                                    entry,
                                    edge.EntryValue
                                        .ToString())
                                .IsCollectionType();

                        var query_getOutEdgesByLabelOfVertexById
                            = GDbGremlinQueries
                                .GetGetOutEdgesByLabelOfVertexByIdQuery(
                                    g,
                                    entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                    edge.EntryName);

                        var currentEdges
                            = query_getOutEdgesByLabelOfVertexById
                                .ToList();

                        if (!ancestorEdgesToCreate
                            .ContainsKey(
                                edge))
                        {
                            ancestorEdgesToCreate[edge] = new List<object>();
                        }

                        if (!ancestorEdgesToDelete
                            .ContainsKey(
                                edge))
                        {
                            ancestorEdgesToDelete[edge] = new List<object>();
                        }

                        if (isMultipleEdge)
                        {
                            var otherEntries
                                = (IEnumerable)
                                    GDbReflectionHelper
                                        .GetValueOfProperty(
                                            entry,
                                            edge.EntryValue
                                                .ToString());

                            if (otherEntries != null)
                            {
                                // regardless whether it was included or not, just overwrite it

                                foreach (var otherEntry in otherEntries)
                                {
                                    if (otherEntry != null)
                                    {
                                        var otherEntryId
                                            = ((Vertex)
                                                otherEntry).TopDatabaseId;

                                        var existingEdge = false;

                                        foreach (var currentEdge in currentEdges)
                                        {
                                            if ((long)
                                                currentEdge.InV.Id == (long)
                                                    otherEntryId)
                                            {
                                                // it is a necessary and existing relationship, nothing to do

                                                existingEdge = true;

                                                currentEdges
                                                    .Remove(
                                                         currentEdge);

                                                break;
                                            }
                                        }

                                        if (!existingEdge)
                                        {
                                            // necessary, but not existing relationship should be created

                                            foreach (var ancestorOfOtherEntry in ((Vertex)
                                                otherEntry).AncestorTypeDatabaseIdMappingCache.Values)
                                            {
                                                var query_addEdgeWithLabelFromVertexByIdToVertexById
                                                    = GDbGremlinQueries
                                                        .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                                            g,
                                                            entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                                            ancestorOfOtherEntry,
                                                            edge.EntryName);

                                                query_addEdgeWithLabelFromVertexByIdToVertexById
                                                    .Next();

                                                ancestorEdgesToCreate[edge]
                                                    .Add(
                                                        ancestorOfOtherEntry);
                                            }
                                        }
                                    }
                                }

                                // unnecessary, but existing relationships should be deleted

                                foreach (var currentEdge in currentEdges)
                                {
                                    var query_dropEdgeById
                                        = GDbGremlinQueries
                                            .GetDropEdgeByIdQuery(
                                                g,
                                                currentEdge.Id);

                                    query_dropEdgeById
                                        .Next();

                                    ancestorEdgesToDelete[edge]
                                        .Add(
                                            currentEdge.InV.Id);
                                }
                            }
                            else
                            {
                                if (entry.LoadedProperties
                                    .Contains(
                                        edge.EntryValue
                                            .ToString()))
                                {
                                    // it had been included, but got deleted, so all of the relationships should be deleted

                                    foreach (var currentEdge in currentEdges)
                                    {
                                        var query_dropEdgeById
                                            = GDbGremlinQueries
                                                .GetDropEdgeByIdQuery(
                                                    g,
                                                    currentEdge.Id);

                                        query_dropEdgeById
                                            .Next();

                                        ancestorEdgesToDelete[edge]
                                            .Add(
                                                currentEdge.InV.Id);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var otherEntry
                                = (Vertex)
                                    GDbReflectionHelper
                                        .GetValueOfProperty(
                                            entry,
                                            edge.EntryValue
                                                .ToString());

                            if (otherEntry != null)
                            {
                                // regardless whether it was included or not, just overwrite it

                                if (currentEdges.Count > 0)
                                {
                                    var edgesDeleted = false;

                                    foreach (var currentEdge in currentEdges)
                                    {
                                        if (!otherEntry.AncestorTypeDatabaseIdMappingCache.Values
                                            .Contains(
                                                currentEdge.InV.Id))
                                        {
                                            // it is not the same as the existing, so it should be deleted

                                            var query_dropEdgeById
                                                = GDbGremlinQueries
                                                    .GetDropEdgeByIdQuery(
                                                        g,
                                                        currentEdge.Id);

                                            query_dropEdgeById
                                                .Next();

                                            ancestorEdgesToDelete[edge]
                                                .Add(
                                                    currentEdge.InV.Id);

                                            edgesDeleted = true;
                                        }
                                    }

                                    if (edgesDeleted)
                                    {
                                        // old edges had been deleted, so new ones should be created

                                        foreach (var ancestorOfOtherEntry in otherEntry.AncestorTypeDatabaseIdMappingCache.Values)
                                        {
                                            var query_addEdgeWithLabelFromVertexByIdToVertexById
                                                = GDbGremlinQueries
                                                    .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                                        g,
                                                        entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                                        ancestorOfOtherEntry,
                                                        edge.EntryName);

                                            query_addEdgeWithLabelFromVertexByIdToVertexById
                                                .Next();

                                            ancestorEdgesToCreate[edge]
                                                .Add(
                                                    ancestorOfOtherEntry);
                                        }
                                    }
                                }
                                else
                                {
                                    // it is a brand new relationship, so it should be created

                                    foreach (var ancestorOfOtherEntry in otherEntry.AncestorTypeDatabaseIdMappingCache.Values)
                                    {
                                        var query_addEdgeWithLabelFromVertexByIdToVertexById
                                            = GDbGremlinQueries
                                                .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                                                    g,
                                                    entry.AncestorTypeDatabaseIdMappingCache[ancestor],
                                                    ancestorOfOtherEntry,
                                                    edge.EntryName);

                                        query_addEdgeWithLabelFromVertexByIdToVertexById
                                            .Next();

                                        ancestorEdgesToCreate[edge]
                                            .Add(
                                                ancestorOfOtherEntry);
                                    }
                                }
                            }
                            else
                            {
                                if (entry.LoadedProperties
                                    .Contains(
                                        edge.EntryValue
                                            .ToString()))
                                {
                                    if (currentEdges.Count > 0)
                                    {
                                        // an existing relationship had been included, but got deleted, so it should be deleted

                                        foreach (var currentEdge in currentEdges)
                                        {
                                            var query_dropEdgeById
                                                = GDbGremlinQueries
                                                    .GetDropEdgeByIdQuery(
                                                        g,
                                                        currentEdge.Id);

                                            query_dropEdgeById
                                                .Next();

                                            ancestorEdgesToDelete[edge]
                                                .Add(
                                                    currentEdge.InV.Id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return updatedVertices;
        }

        public Task<int> UpdateAsync(
            IEnumerable<IUpdateEntry> updateEntries, 
            CancellationToken cancellationToken = default(CancellationToken))
            => Task
                .FromResult(
                    Update(
                        updateEntries));

        private long AddOrUpdatePropertyToOrOfVertex(
            object vertexId,
            string name,
            object value)
        {
            // retrieving property vertex's id if exists

            var query_getOutNeighboursByEdgeLabelOfVertexById
                = GDbGremlinQueries
                    .GetGetOutNeighboursByEdgeLabelOfVertexByIdQuery(
                        g,
                        vertexId,
                        name);

            var neighbours
                = query_getOutNeighboursByEdgeLabelOfVertexById
                    .Id()
                    .ToList();

            if (neighbours.Count == 1)
            {
                var id
                    = long
                        .Parse(
                            neighbours
                                .First()
                                .ToString());

                // updating the property's value

                var query_addOrUpdatePropertyToOrOfVertexByIdQuery
                    = GDbGremlinQueries
                        .GetAddOrUpdatePropertyToOrOfVertexByIdQuery(
                            g,
                            id,
                            GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                            value);

                query_addOrUpdatePropertyToOrOfVertexByIdQuery
                    .Next();

                return id;
            }
            else if (neighbours.Count == 0)
            {
                // adding a new property

                return AddPropertyToVertex(
                    vertexId,
                    name,
                    value);
            }
            else
            {
                throw new GDbDatabaseIntegrityDamagedException(
                    "A vertex has more than one '" + name + "' property in the database.");
            }
        }

        private long AddPropertyToVertex(
            object vertexId,
            string name,
            object value)
        {
            // adding vertex for property

            var query_addVertexWithLabel
                = GDbGremlinQueries
                    .GetAddVertexWithLabelQuery(
                        g,
                        GDbModelConstants._INNER_PROPERTY_VERTEX_NAME)
                    .Id();

            var id
                = long
                    .Parse(
                        query_addVertexWithLabel
                            .Next()
                            .ToString());

            // setting property's value

            var query_addOrUpdatePropertyToOrOfVertexById
                = GDbGremlinQueries
                    .GetAddOrUpdatePropertyToOrOfVertexByIdQuery(
                        g,
                        id,
                        GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                        value);

            query_addOrUpdatePropertyToOrOfVertexById
                .Next();

            // connecting vertex with property

            var query_addEdgeWithLabelFromVertexByIdToVertexById
                = GDbGremlinQueries
                    .GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
                        g,
                        vertexId,
                        id,
                        name);

            query_addEdgeWithLabelFromVertexByIdToVertexById
                .Next();

            return id;
        }

        private GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> BuildQueryPartOrdering(
            Type type,
            GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> queryBase,
            GDbQueryOperationOrderBy gDbQueryOperationOrderBy,
            IEnumerable<GDbQueryOperationThenBy> gDbQueryOperationThenBys)
        {
            if (gDbQueryOperationOrderBy != null)
            {
                var query
                    = queryBase
                        .Order()
                        .By(
                            BuildQueryPartPropertySelector(
                                type,
                                (GDbQueryOperationPropertySelector)
                                    gDbQueryOperationOrderBy.Argument1)
                            .As(
                                "property_vertex")
                            .Select<object>(
                                "property_vertex")
                            .By(
                                GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME),
                            (bool)
                                RetrieveConstantValueFromGDbQueryOperationConstant(
                                    (GDbQueryOperationConstant)
                                        gDbQueryOperationOrderBy.Argument2) == true ? Order.Decr : Order.Incr);

                foreach (var gDbQueryOperationThenBy in gDbQueryOperationThenBys)
                {
                    query
                        = query
                            .By(
                                BuildQueryPartPropertySelector(
                                    type,
                                    (GDbQueryOperationPropertySelector)
                                        gDbQueryOperationThenBy.Argument1)
                                .As(
                                    "property_vertex")
                                .Select<object>(
                                    "property_vertex")
                                .By(
                                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME),
                                (bool)
                                    RetrieveConstantValueFromGDbQueryOperationConstant(
                                        (GDbQueryOperationConstant)
                                            gDbQueryOperationThenBy.Argument2) == true ? Order.Decr : Order.Incr);
                }

                return query;
            }
            else
            {
                return queryBase;
            }
        }

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartPropertySelector(
            Type type,
            GDbQueryOperationPropertySelector gDbQueryOperationPropertySelector)
        {
            GraphTraversal<object, Gremlin.Net.Structure.Vertex> query;

            var propertyName
                = RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                    gDbQueryOperationPropertySelector)
                    .Replace(
                        "\"",
                        "");

            if (propertyName != "Id" && propertyName
                .EndsWith(
                   "Id"))
            {
                propertyName 
                    = propertyName
                        .Replace(
                            "Id",
                            ".Id");
            }

            var pathParts
                = propertyName
                    .Split(
                        '.');

            var relationshipTuple
                = GetRelationshipTuple(
                    type,
                    pathParts[0]);

            if (relationshipTuple.Item1)
            {
                query
                    = __
                        .Out(
                            relationshipTuple.Item2);
            }
            else
            {
                query
                    = __
                        .In(
                            relationshipTuple.Item2);
            }

            type =
                GDbReflectionHelper
                    .GetTypeOfProperty(
                        type,
                        pathParts[0]);

            if (type
                .IsCollectionType())
            {
                type
                    = type
                        .GetCollectionType();
            }

            for (var i = 1; i != pathParts
                .Count(); i++)
            {
                relationshipTuple
                    = GetRelationshipTuple(
                        type,
                        pathParts[i]);

                if (relationshipTuple.Item1)
                {
                    query
                        = query
                            .Out(
                                relationshipTuple.Item2);
                }
                else
                {
                    query
                        = query
                            .In(
                                relationshipTuple.Item2);
                }

                type =
                    GDbReflectionHelper
                        .GetTypeOfProperty(
                            type,
                            pathParts[i]);

                if (type
                    .IsCollectionType())
                {
                    type
                        = type
                            .GetCollectionType();
                }
            }

            return query;
        }

        private GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> BuildQueryPartPropertySelector(
            Type type,
            GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> queryBase,
            GDbQueryOperationPropertySelector gDbQueryOperationPropertySelector)
        {
            GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> query;

            var propertyName
                = RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                    gDbQueryOperationPropertySelector)
                    .Replace(
                        "\"",
                        "");

            if (propertyName != "Id" && propertyName
                .EndsWith(
                   "Id"))
            {
                propertyName
                    = propertyName
                        .Replace(
                            "Id",
                            ".Id");
            }

            var pathParts
                = propertyName
                    .Split(
                        '.');

            var relationshipTuple
                = GetRelationshipTuple(
                    type,
                    pathParts[0]);

            if (relationshipTuple.Item1)
            {
                query
                    = queryBase
                        .Out(
                            relationshipTuple.Item2);
            }
            else
            {
                query
                    = queryBase
                        .In(
                            relationshipTuple.Item2);
            }

            type =
                GDbReflectionHelper
                    .GetTypeOfProperty(
                        type,
                        pathParts[0]);

            if (type
                .IsCollectionType())
            {
                type
                    = type
                        .GetCollectionType();
            }

            for (var i = 1; i != pathParts
                .Count(); i++)
            {
                relationshipTuple
                    = GetRelationshipTuple(
                        type,
                        pathParts[i]);

                if (relationshipTuple.Item1)
                {
                    query
                        = query
                            .Out(
                                relationshipTuple.Item2);
                }
                else
                {
                    query
                        = query
                            .In(
                                relationshipTuple.Item2);
                }

                type =
                    GDbReflectionHelper
                        .GetTypeOfProperty(
                            type,
                            pathParts[i]);

                if (type
                    .IsCollectionType())
                {
                    type
                        = type
                            .GetCollectionType();
                }
            }

            return query;
        }

        private GraphTraversal<Gremlin.Net.Structure.Vertex, object> BuildQueryPartSelect(
            GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> queryBase,
            IEnumerable<GDbQueryOperationPropertySelector> gDbQueryOperationPropertySelectors)
        {
            if (gDbQueryOperationPropertySelectors == null)
            {
                return queryBase
                    .Out()
                    .HasLabel(
                        GDbModelConstants._INNER_PROPERTY_VERTEX_NAME)
                    .As(
                        "property_vertex")
                    .Select<object>(
                        "property_vertex")
                    .By(
                        GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME)
                    .Dedup()
                    .Unfold<object>();
            }
            else
            {
                var properties
                    = gDbQueryOperationPropertySelectors
                        .Select(
                            o => RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                                o))
                        .ToArray();

                return queryBase
                    .Out(
                        properties)
                    .As(
                        "property_vertex")
                    .Select<object>(
                        "property_vertex")
                    .By(
                        GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME)
                    .Dedup()
                    .Unfold<object>();
            }
        }

        private GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> BuildQueryPartWhere(
            Type type,
            GraphTraversal<Gremlin.Net.Structure.Vertex, Gremlin.Net.Structure.Vertex> queryBase,
            GDbQueryOperationWhere gDbQueryOperationWhere)
        {
            if (gDbQueryOperationWhere == null)
            {
                // there are not where clauses, so all the entities are wanted

                return queryBase;
            }
            else
            {
                if (gDbQueryOperationWhere.Argument is GDbQueryOperationLogical || gDbQueryOperationWhere.Argument is GDbQueryOperationRelational)
                {
                    return queryBase
                        .Where(
                            BuildQueryPartWherePart(
                                type,
                                gDbQueryOperationWhere.Argument));
                }
                else
                {
                    throw new InvalidOperationException(
                        "'WHERE' clause must contain at least one relational (==, >, >=, <, <=, !=) expression and some (0 or more) logical expressions (&&, ^, !, ||).");
                }
            }
        }

        private object BuildQueryPartWherePart(
            Type type,
            IGDbQueryOperationArgument gDbQueryOperationArgument)
        {
            var gDbQueryOperation
                = (GDbQueryOperation)
                    gDbQueryOperationArgument;

            if (gDbQueryOperation is GDbQueryOperationLogical)
            {
                if (!gDbQueryOperation
                    .GetArguments()
                    .All(
                        o => o is GDbQueryOperationLogical || o is GDbQueryOperationRelational || o == null))
                {
                    throw new InvalidOperationException(
                        "Both arguments of a logical expression must be a logical or relational expression.");
                }
            }

            if (gDbQueryOperation is GDbQueryOperationRelational)
            {
                if (!(gDbQueryOperation
                    .GetArguments()
                    .ElementAt(
                        0) is GDbQueryOperationPropertySelector) || !((gDbQueryOperation
                            .GetArguments()
                            .ElementAt(
                                1) is GDbQueryOperationArithmetic) || !(gDbQueryOperation
                                    .GetArguments()
                                    .ElementAt(
                                        1) is GDbQueryOperationConstant) || !(gDbQueryOperation
                                            .GetArguments()
                                            .ElementAt(
                                                1) is GDbQueryOperationPropertySelector)))
                {
                    throw new InvalidOperationException(
                        "Both arguments of a relational expression must be a logical or relational expression.");
                }
            }

            switch (gDbQueryOperation
                .GetName())
            {
                case GDbQueryOperationNames.LOGICAL_AND:
                    return BuildQueryPartWherePartLogicalAnd(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.LOGICAL_EXCLUSIVE_OR:
                    return BuildQueryPartWherePartLogicalExclusiveOr(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.LOGICAL_NOT:
                    return BuildQueryPartWherePartLogicalNot(
                        type,
                        gDbQueryOperation
                            .GetArguments()
                            .First());
                case GDbQueryOperationNames.LOGICAL_OR:
                    return BuildQueryPartWherePartLogicalOr(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.RELATIONAL_EQUAL:
                    return BuildQueryPartWherePartRelationalEqual(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.RELATIONAL_GREATER_THAN:
                    return BuildQueryPartWherePartRelationalGreaterThan(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.RELATIONAL_GREATER_THAN_OR_EQUAL:
                    return BuildQueryPartWherePartRelationalGreaterThanOrEqual(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.RELATIONAL_LESS_THAN:
                    return BuildQueryPartWherePartRelationalLessThan(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.RELATIONAL_LESS_THAN_OR_EQUAL:
                    return BuildQueryPartWherePartRelationalLessThanOrEqual(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                case GDbQueryOperationNames.RELATIONAL_NOT_EQUAL:
                    return BuildQueryPartWherePartRelationalNotEqual(
                        type,
                        gDbQueryOperation
                            .GetArguments());
                default:
                    return null;
            }
        }

        private object BuildQueryPartWherePartArithmetic(
            GDbQueryOperationArithmetic gDbQueryOperationArithmetic)
        {
            if (!gDbQueryOperationArithmetic
                .GetArguments()
                .All(
                    o => o is GDbQueryOperationConstant))
            {
                throw new InvalidOperationException(
                    "Both arguments of an arithmetic expression must be a constant value or a variable.");
            }

            var left
                = RetrieveConstantValueFromGDbQueryOperationConstant(
                    (GDbQueryOperationConstant)
                        gDbQueryOperationArithmetic.Argument1);

            var right
                = RetrieveConstantValueFromGDbQueryOperationConstant(
                    (GDbQueryOperationConstant)
                        gDbQueryOperationArithmetic.Argument2);

            switch (gDbQueryOperationArithmetic
                .GetName())
            {
                case GDbQueryOperationNames.ARITHMETIC_ADD:
                    return BuildQueryPartWherePartArithmeticAdd(
                        left,
                        right);
                case GDbQueryOperationNames.ARITHMETIC_DIVIDE:
                    return BuildQueryPartWherePartArithmeticDivide(
                        left,
                        right);
                case GDbQueryOperationNames.ARITHMETIC_MULTIPLY:
                    return BuildQueryPartWherePartArithmeticMultiply(
                        left,
                        right);
                case GDbQueryOperationNames.ARITHMETIC_SUBTRACT:
                    return BuildQueryPartWherePartArithmeticSubtract(
                        left,
                        right);
                default:
                    return null;
            }
        }

        private double BuildQueryPartWherePartArithmeticAdd(
            object argument1,
            object argument2)
            => (double)
                argument1 + (double)
                    argument2;

        private double BuildQueryPartWherePartArithmeticDivide(
            object argument1,
            object argument2)
            => (double)
                argument1 / (double)
                    argument2;

        private double BuildQueryPartWherePartArithmeticMultiply(
            object argument1,
            object argument2)
            => (double)
                argument1 * (double)
                    argument2;

        private double BuildQueryPartWherePartArithmeticSubtract(
            object argument1,
            object argument2)
            => (double)
                argument1 - (double)
                    argument2;

        private GraphTraversal<object, object> BuildQueryPartWherePartLogicalAnd(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => __
                .And(
                    BuildQueryPartWherePart(
                        type,
                        gDbQueryOperationArguments
                            .ElementAt(
                                0)),
                    BuildQueryPartWherePart(
                        type,
                        gDbQueryOperationArguments
                            .ElementAt(
                                1)));

        private GraphTraversal<object, object> BuildQueryPartWherePartLogicalExclusiveOr(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
        {
            var left
                = BuildQueryPartWherePart(
                    type,
                    gDbQueryOperationArguments
                        .ElementAt(
                            0));

            var right
                = BuildQueryPartWherePart(
                    type,
                    gDbQueryOperationArguments
                        .ElementAt(
                            1));

            return __
                .And(
                    __
                        .Or(
                            left,
                            right),
                    __
                        .Or(
                            __
                                .Not(
                                    left),
                            __
                                .Not(
                                    right)));
        }

        private GraphTraversal<object, object> BuildQueryPartWherePartLogicalNot(
            Type type,
            IGDbQueryOperationArgument gDbQueryOperationArgument)
            => __
                .Not(
                    BuildQueryPartWherePart(
                        type,
                        gDbQueryOperationArgument));

        private GraphTraversal<object, object> BuildQueryPartWherePartLogicalOr(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => __
                .Or(
                    BuildQueryPartWherePart(
                        type,
                        gDbQueryOperationArguments
                            .ElementAt(
                                0)),
                    BuildQueryPartWherePart(
                        type,
                        gDbQueryOperationArguments
                            .ElementAt(
                                1)));

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartWherePartRelationalEqual(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => BuildQueryPartPropertySelector(
                type,
                (GDbQueryOperationPropertySelector)
                    gDbQueryOperationArguments
                        .ElementAt(
                            0))
                .Has(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                    P
                        .Eq(
                            BuildQueryPartWherePartRelationalRightOperand(
                                type,
                                gDbQueryOperationArguments
                                    .ElementAt(
                                        1))));

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartWherePartRelationalGreaterThan(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => BuildQueryPartPropertySelector(
                type,
                (GDbQueryOperationPropertySelector)
                    gDbQueryOperationArguments
                        .ElementAt(
                            0))
                .Has(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                    P
                        .Gt(
                            BuildQueryPartWherePartRelationalRightOperand(
                                type,
                                gDbQueryOperationArguments
                                    .ElementAt(
                                        1))));

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartWherePartRelationalGreaterThanOrEqual(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => BuildQueryPartPropertySelector(
                type,
                (GDbQueryOperationPropertySelector)
                    gDbQueryOperationArguments
                        .ElementAt(
                            0))
                .Has(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                    P
                        .Gte(
                            BuildQueryPartWherePartRelationalRightOperand(
                                type,
                                gDbQueryOperationArguments
                                    .ElementAt(
                                        1))));

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartWherePartRelationalLessThan(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => BuildQueryPartPropertySelector(
                type,
                (GDbQueryOperationPropertySelector)
                    gDbQueryOperationArguments
                        .ElementAt(
                            0))
                .Has(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                    P
                        .Lt(
                            BuildQueryPartWherePartRelationalRightOperand(
                                type,
                                gDbQueryOperationArguments
                                    .ElementAt(
                                        1))));

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartWherePartRelationalLessThanOrEqual(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => BuildQueryPartPropertySelector(
                type,
                (GDbQueryOperationPropertySelector)
                    gDbQueryOperationArguments
                        .ElementAt(
                            0))
                .Has(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                    P
                        .Lte(
                            BuildQueryPartWherePartRelationalRightOperand(
                                type,
                                gDbQueryOperationArguments
                                    .ElementAt(
                                        1))));

        private GraphTraversal<object, Gremlin.Net.Structure.Vertex> BuildQueryPartWherePartRelationalNotEqual(
            Type type,
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments)
            => BuildQueryPartPropertySelector(
                type,
                (GDbQueryOperationPropertySelector)
                    gDbQueryOperationArguments
                        .ElementAt(
                            0))
                .Has(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME,
                    P
                        .Neq(
                            BuildQueryPartWherePartRelationalRightOperand(
                                type,
                                gDbQueryOperationArguments
                                    .ElementAt(
                                        1))));

        private object BuildQueryPartWherePartRelationalRightOperand(
            Type type,
            IGDbQueryOperationArgument gDbQueryOperationArgument)
        {
            if (gDbQueryOperationArgument is GDbQueryOperationArithmetic)
            {
                return BuildQueryPartWherePartArithmetic(
                    (GDbQueryOperationArithmetic)
                        gDbQueryOperationArgument);
            }
            else if (gDbQueryOperationArgument is GDbQueryOperationConstant)
            {
                return RetrieveConstantValueFromGDbQueryOperationConstant(
                    (GDbQueryOperationConstant)
                        gDbQueryOperationArgument);
            }
            else if (gDbQueryOperationArgument is GDbQueryOperationPropertySelector)
            {
                return BuildQueryPartPropertySelector(
                    type,
                    null,
                    (GDbQueryOperationPropertySelector)
                        gDbQueryOperationArgument);
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<Type> CollectAncestorsOfType(
            Type type)
        {
            IList<Type> ancestors = new List<Type>();

            ancestors
                .Add(
                    type);

            var mainModelDescriptor
                = ContextOptionsExtension
                    .GetModelDescriptor(
                        type);

            if (mainModelDescriptor != null)
            {
                var directAncestors
                    = mainModelDescriptor
                        .GetModelDescriptorEntriesByType(
                            GDbModelDescriptorEntryTypes.ANCESTOR);

                foreach (var directAncestor in directAncestors)
                {
                    if (!ancestors
                        .Contains(
                            (Type)
                                directAncestor.EntryValue))
                    {
                        ancestors
                            .Add(
                                (Type)
                                    directAncestor.EntryValue);
                    }
                }

                var i = 0;

                while (i < ancestors.Count)
                {
                    var ancestorsModelDescriptor
                        = ContextOptionsExtension
                            .GetModelDescriptor(
                                ancestors[i]);

                    if (ancestorsModelDescriptor != null)
                    {
                        var ancestorsOfAncestor
                            = ancestorsModelDescriptor
                                .GetModelDescriptorEntriesByType(
                                    GDbModelDescriptorEntryTypes.ANCESTOR);

                        foreach (var ancestorOfAncestor in ancestorsOfAncestor)
                        {
                            if (!ancestors
                                .Contains(
                                    (Type)
                                        ancestorOfAncestor.EntryValue))
                            {
                                ancestors
                                    .Add(
                                        (Type)
                                            ancestorOfAncestor.EntryValue);
                            }
                        }
                    }

                    i++;
                }
            }

            return ancestors;
        }

        private IDictionary<Type, IEnumerable<Type>> CreateAncestorCache(
            IEnumerable<IUpdateEntry> updateEntries)
        {
            IDictionary<Type, IEnumerable<Type>> ancestorsCache = new Dictionary<Type, IEnumerable<Type>>();

            foreach (var updateEntry in updateEntries)
            {
                if (updateEntry != null)
                {
                    if (!ancestorsCache
                        .ContainsKey(
                            updateEntry.EntityType.ClrType))
                    {
                        ancestorsCache[updateEntry.EntityType.ClrType] =
                            CollectAncestorsOfType(
                                updateEntry.EntityType.ClrType);
                    }
                }
            }

            return ancestorsCache;
        }

        private IDictionary<Type, IEnumerable<Type>> CreateAncestorCache(
            IEnumerable<Type> types)
        {
            IDictionary<Type, IEnumerable<Type>> ancestorsCache = new Dictionary<Type, IEnumerable<Type>>();

            foreach (var type in types)
            {
                if (type != null)
                {
                    if (!ancestorsCache
                        .ContainsKey(
                            type))
                    {
                        ancestorsCache[type] =
                            CollectAncestorsOfType(
                                type);
                    }
                }
            }

            return ancestorsCache;
        }

        private string GetLabelOfType(
            Type type)
        {
            var labels
                = ContextOptionsExtension
                    .GetModelDescriptor(
                        type)
                    .GetModelDescriptorEntriesByType(
                        GDbModelDescriptorEntryTypes.LABEL);

            return labels.Count == 0
                ? type.Name
                : labels
                    .First().EntryValue
                        .ToString();
        }

        private Tuple<bool, string> GetRelationshipTuple(
            Type type,
            string relationshipName)
        {
            // true means out relationship, false means in relationship

            var ancestorCache
                = CreateAncestorCache(
                    new Type[]
                    {
                        type
                    });

            foreach (var ancestor in ancestorCache[type]
                .Reverse())
            {
                var modelDescriptor
                    = ContextOptionsExtension
                        .GetModelDescriptor(
                            ancestor);

                if (modelDescriptor
                    .HasProperty(
                        relationshipName))
                {
                    foreach (var gDbModelDescriptorEntry in modelDescriptor
                        .GetModelDescriptorEntriesByType(
                            GDbModelDescriptorEntryTypes.PROPERTY))
                    {
                        if (gDbModelDescriptorEntry.EntryValue
                            .ToString() == relationshipName)
                        {
                            return new Tuple<bool, string>(
                                true,
                                gDbModelDescriptorEntry.EntryName);
                        }
                    }
                }
                else if (modelDescriptor
                    .HasEdge(
                        relationshipName))
                {
                    foreach (var gDbModelDescriptorEntry in modelDescriptor
                        .GetModelDescriptorEntriesByType(
                            GDbModelDescriptorEntryTypes.EDGE))
                    {
                        if (gDbModelDescriptorEntry.EntryValue
                            .ToString() == relationshipName)
                        {
                            return new Tuple<bool, string>(
                                true,
                                gDbModelDescriptorEntry.EntryName);
                        }
                    }
                }
                else
                {
                    var typeOfProperty
                        = GDbReflectionHelper
                            .GetTypeOfProperty(
                                type,
                                relationshipName);

                    modelDescriptor
                        = ContextOptionsExtension
                            .GetModelDescriptor(
                                typeOfProperty);

                    if (modelDescriptor != null)
                    {
                        foreach (var gDbModelDescriptorEntry in modelDescriptor
                            .GetModelDescriptorEntriesByType(
                                GDbModelDescriptorEntryTypes.EDGE))
                        {
                            var typeOfOtherProperty
                                = GDbReflectionHelper
                                    .GetTypeOfProperty(
                                        typeOfProperty,
                                        gDbModelDescriptorEntry.EntryValue
                                            .ToString());

                            if (typeOfOtherProperty
                                .IsCollectionType())
                            {
                                typeOfOtherProperty
                                    = typeOfOtherProperty
                                        .GetCollectionType();
                            }

                            if (gDbModelDescriptorEntry.Extra != null && gDbModelDescriptorEntry.Extra
                                .ToString() == relationshipName && typeOfOtherProperty
                                    .IsAssignableFrom(
                                        type))
                            {
                                return new Tuple<bool, string>(
                                    false,
                                    gDbModelDescriptorEntry.EntryName);
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException(
                "Type '" + type.Name + "' has no '" + relationshipName + "' relationship mapped in the database.");
        }

        private object GetValueOfProperty(
            Type rootType,
            object id,
            string propertyName)
            => BuildQueryPartPropertySelector(
                rootType,
                g
                    .V(
                        id),
                new GDbQueryOperationPropertySelector(
                    propertyName))
                .As(
                    "property_vertex")
                .Select<object>(
                    "property_vertex")
                .By(
                    GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME)
                .Unfold<object>()
                .ToList()
                .FirstOrDefault();

        private TEntity MaterializeEntity<TEntity>(
            Type rootType,
            object id,
            IEnumerable<GDbQueryOperationPropertySelector> gDbQueryOperationPropertySelectors,
            IEnumerable<GDbQueryOperationIncludeChain> gDbQueryOperationIncludeChains)
        {
            var ancestorCache
                = CreateAncestorCache(
                    new Type[]
                    {
                        rootType
                    });

            /*
             * ancestors
             */

            IDictionary<string, string> propertyNameMappingCache = new Dictionary<string, string>();

            foreach (var ancestor in ancestorCache[rootType]
                .Reverse())
            {
                var rootModelDescriptor
                    = ContextOptionsExtension
                        .GetModelDescriptor(
                            ancestor);

                foreach (var gDbModelDescriptorEntry in rootModelDescriptor
                    .GetModelDescriptorEntriesByType(
                        GDbModelDescriptorEntryTypes.PROPERTY))
                {
                    propertyNameMappingCache[gDbModelDescriptorEntry.EntryName]
                        = gDbModelDescriptorEntry.EntryValue
                            .ToString();
                }
            }

            /*
             * selected properties
             */

            IList<GDbQueryOperationPropertySelector> listOfPropertySelectors = null;

            if (gDbQueryOperationPropertySelectors == null)
            {
                listOfPropertySelectors
                    = propertyNameMappingCache.Keys
                        .Select(
                            o => new GDbQueryOperationPropertySelector(
                                o))
                        .ToList();
            }
            else
            {
                listOfPropertySelectors
                    = gDbQueryOperationPropertySelectors
                        .ToList();
            }

            if (typeof(TEntity)
                .IsSimpleType())
            {
                // result is a simple (and single) object

                return (TEntity)
                    BuildQueryPartPropertySelector(
                        rootType,
                        g
                            .V(
                                id),
                        listOfPropertySelectors
                            .First())
                        .As(
                            "property_vertex")
                        .Select<object>(
                            "property_vertex")
                        .By(
                            GDbModelConstants._INNER_PROPERTY_VERTEX_VALUE_NAME)
                        .Unfold<object>()
                        .Next();
            }
            else
            {
                // result's type is complex

                if (typeof(TEntity)
                    .IsAnonymousType())
                {
                    // anonymous type result

                    var constructorParameters = new object[listOfPropertySelectors.Count];

                    for (var i = 0; i != listOfPropertySelectors.Count; i++)
                    {
                        constructorParameters[i]
                            = GetValueOfProperty(
                                rootType,
                                id,
                                RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                                    listOfPropertySelectors[i]));
                    }

                    return (TEntity) 
                        Activator
                            .CreateInstance(
                                typeof(TEntity),
                                constructorParameters);
                }
                else
                {
                    // well-known type result

                    TEntity entity
                        = (TEntity)
                            Activator
                                .CreateInstance(
                                    typeof(TEntity));

                    var loadedPropertiesProperty
                        = (ISet<string>)
                            entity
                                .GetType()
                                .GetProperty(
                                    nameof(Vertex.LoadedProperties))
                                .GetValue(
                                    entity);

                    var ancestorTypeDatabaseIdMappingCacheProperty
                        = (IDictionary<Type, object>)
                            entity
                                .GetType()
                                .GetProperty(
                                    nameof(Vertex.AncestorTypeDatabaseIdMappingCache))
                                .GetValue(
                                    entity);

                    /*
                     * requested properties
                     */

                    foreach (var propertySelector in listOfPropertySelectors)
                    {
                        var propertyName
                            = RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                                propertySelector);

                        GDbReflectionHelper
                            .SetValueOfProperty(
                                entity,
                                propertyNameMappingCache[propertyName],
                                GetValueOfProperty(
                                    rootType,
                                    id,
                                    propertyNameMappingCache[propertyName]));

                        loadedPropertiesProperty
                            .Add(
                                propertyNameMappingCache[propertyName]);
                    }

                    /*
                     * requested relationships (includes)
                     */

                    foreach (var gDbQueryOperationIncludeChain in gDbQueryOperationIncludeChains)
                    {
                        var propertySelectors
                            = gDbQueryOperationIncludeChain
                                .GetArguments()
                                .Cast<GDbQueryOperationPropertySelector>();

                        if (propertySelectors
                            .Count() == 0)
                        {
                            continue;
                        }

                        var propertySelector
                            = propertySelectors
                                .ElementAt(
                                    0);

                        var forwardablePropertySelectors
                            = propertySelectors
                                .Count() > 1
                                    ? new GDbQueryOperationIncludeChain[]
                                        {
                                        new GDbQueryOperationIncludeChain(
                                            propertySelectors
                                                .Skip(
                                                    1)
                                                .Select(
                                                    o => RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                                                        o))
                                                .ToArray())
                                        }
                                    : new GDbQueryOperationIncludeChain[]
                                        {

                                        };

                        var propertyName
                            = RetrievePropertyNameFromGDBQueryOperationPropertySelector(
                                propertySelector);

                        var typeOfProperty
                            = GDbReflectionHelper
                                .GetTypeOfProperty(
                                    rootType,
                                    propertyName);

                        if (typeOfProperty
                            .IsCollectionType())
                        {
                            typeOfProperty
                                = typeOfProperty
                                    .GetCollectionType();

                            var query
                                = BuildQueryPartPropertySelector(
                                    rootType,
                                    g
                                        .V(
                                            id),
                                    propertySelector)
                                .HasLabel(
                                    GetLabelOfType(
                                        typeOfProperty))
                                .Id();

                            var relatedIds
                                = query
                                    .ToList();

                            var collectionType
                                = typeof(Collection<>)
                                    .MakeGenericType(
                                        typeOfProperty);

                            var collection
                                = Activator
                                    .CreateInstance(
                                        collectionType);

                            var addMethod
                                = collectionType
                                    .GetMethod(
                                        "Add");

                            foreach (var relatedId in relatedIds)
                            {
                                addMethod
                                    .Invoke(
                                        collection,
                                        new object[]
                                            {
                                            typeof(GDbGremlinQueryExecutor)
                                                .GetTypeInfo()
                                                .GetDeclaredMethod(
                                                    nameof(MaterializeEntity))
                                                .MakeGenericMethod(
                                                    typeOfProperty)
                                                .Invoke(
                                                    this,
                                                    new object[]
                                                        {
                                                            typeOfProperty,
                                                            relatedId,
                                                            null,
                                                            forwardablePropertySelectors
                                                        })
                                            });
                            }

                            GDbReflectionHelper
                                .SetValueOfProperty(
                                    entity,
                                    propertyName,
                                    collection);

                            loadedPropertiesProperty
                                .Add(
                                    propertyName);
                        }
                        else
                        {
                            var query
                                = BuildQueryPartPropertySelector(
                                    rootType,
                                    g
                                        .V(
                                            id),
                                    propertySelector)
                                .HasLabel(
                                    GetLabelOfType(
                                        typeOfProperty))
                                .Id();

                            var relatedIds
                                = query
                                    .ToList();

                            if (relatedIds.Count > 0)
                            {
                                GDbReflectionHelper
                                    .SetValueOfProperty(
                                        entity,
                                        propertyName,
                                        (typeof(GDbGremlinQueryExecutor)
                                            .GetTypeInfo()
                                            .GetDeclaredMethod(
                                                nameof(MaterializeEntity))
                                            .MakeGenericMethod(
                                                typeOfProperty)
                                            .Invoke(
                                                this,
                                                new object[]
                                                    {
                                                    typeOfProperty,
                                                    relatedIds
                                                        .Last(),
                                                    null,
                                                    forwardablePropertySelectors
                                                    })));

                                loadedPropertiesProperty
                                    .Add(
                                        propertyName);
                            }
                        }
                    }

                    foreach (var ancestor in ancestorCache[rootType]
                        .Reverse())
                    {
                        var query
                            = g
                                .V(
                                    id)
                                .Out(
                                    GDbModelConstants._INNER_APPLICATION_ID_NAME)
                                .In(
                                    GDbModelConstants._INNER_APPLICATION_ID_NAME)
                                .HasLabel(
                                    GetLabelOfType(
                                        ancestor))
                                .Id();

                        ancestorTypeDatabaseIdMappingCacheProperty[ancestor]
                            = query
                                .Next();
                    }

                    return entity;
                }
            }
        }

        private object RetrieveConstantValueFromGDbQueryOperationConstant(
            GDbQueryOperationConstant gDbQueryOperationConstant)
            => gDbQueryOperationConstant.Value;

        private string RetrievePropertyNameFromGDBQueryOperationPropertySelector(
            GDbQueryOperationPropertySelector gDbQueryOperationPropertySelector)
            => RetrieveConstantValueFromGDbQueryOperationConstant(
                    (GDbQueryOperationConstant)
                        gDbQueryOperationPropertySelector.Argument)
                .ToString();

        #endregion

        #region Fields

        private readonly GraphTraversalSource g;

        private readonly Gremlin.Net.Structure.Graph _graph = new Gremlin.Net.Structure.Graph();

        #endregion
    }
}
