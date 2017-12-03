using LINQtoGremlin.Core.Query.IntermediateTree;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Graph.LanguageSpecific
{
    public interface IGDbQueryExecutor
    {
        int Create(
            IEnumerable<IUpdateEntry> updateEntries);

        Task<int> CreateAsync(
            IEnumerable<IUpdateEntry> updateEntries,
            CancellationToken cancellationToken = default(CancellationToken));

        int Delete(
            IEnumerable<IUpdateEntry> updateEntries);

        Task<int> DeleteAsync(
            IEnumerable<IUpdateEntry> updateEntries,
            CancellationToken cancellationToken = default(CancellationToken));

        IEnumerable<TEntity> Read<TEntity>(
            Type rootType,
            IEnumerable<IGDbQueryOperation> intermediateTree);

        Task<IEnumerable<TEntity>> ReadAsync<TEntity>(
            Type rootType,
            IEnumerable<IGDbQueryOperation> intermediateTree,
            CancellationToken cancellationToken = default(CancellationToken));

        int Update(
            IEnumerable<IUpdateEntry> updateEntries);

        Task<int> UpdateAsync(
            IEnumerable<IUpdateEntry> updateEntries,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
