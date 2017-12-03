using LINQtoGremlin.Core.Query.IntermediateTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public interface IGDbStore
    {
        bool EnsureCreated();

        bool EnsureDeleted();

        Task<IEnumerable<TEntity>> ExecuteEntityQueryTransactionAsync<TEntity>(
            Type rootType,
            IEnumerable<IGDbQueryOperation> gDbQueryOperations,
            IDiagnosticsLogger<DbLoggerCategory.Query> diagnosticsLogger,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<int> ExecuteModifyingTransactionAsync(
            IEnumerable<IUpdateEntry> updateEntries,
            IDiagnosticsLogger<DbLoggerCategory.Update> diagnosticsLogger,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
