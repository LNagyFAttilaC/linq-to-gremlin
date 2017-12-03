using LINQtoGremlin.Core.Extensions.Internal;
using LINQtoGremlin.Core.Graph.LanguageSpecific;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LINQtoGremlin.Core.Query.IntermediateTree;
using System;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbStore 
        : IGDbStore
    {
        #region Constructors

        public GDbStore(
            IGDbQueryExecutorFactory gDbQueryExecutorFactory)
            => _gDbQueryExecutorFactory = gDbQueryExecutorFactory;

        #endregion

        #region Methods

        public virtual bool EnsureCreated()
        {
            return true;
        }

        public virtual bool EnsureDeleted()
        {
            return true;
        }

        public virtual async Task<IEnumerable<TEntity>> ExecuteEntityQueryTransactionAsync<TEntity>(
            Type rootType,
            IEnumerable<IGDbQueryOperation> gDbQueryOperations, 
            IDiagnosticsLogger<DbLoggerCategory.Query> diagnosticsLogger, 
            CancellationToken cancellationToken = default(CancellationToken))
            => await _gDbQueryExecutorFactory
                .Create()
                .ReadAsync<TEntity>(
                    rootType,
                    gDbQueryOperations);

        public virtual async Task<int> ExecuteModifyingTransactionAsync(
            IEnumerable<IUpdateEntry> updateEntries, 
            IDiagnosticsLogger<DbLoggerCategory.Update> diagnosticsLogger,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var entriesAffected = 0;

            var gDbQueryExecutor
                = _gDbQueryExecutorFactory
                    .Create();

            entriesAffected += await gDbQueryExecutor
                .CreateAsync(
                    updateEntries
                        .Where(
                            ue => ue.EntityState == EntityState.Added)
                        .ToList(),
                    cancellationToken);

            entriesAffected += await gDbQueryExecutor
                .DeleteAsync(
                    updateEntries
                        .Where(
                            ue => ue.EntityState == EntityState.Deleted)
                        .ToList(),
                    cancellationToken);

            entriesAffected += await gDbQueryExecutor
                .UpdateAsync(
                    updateEntries
                        .Where(
                            ue => ue.EntityState == EntityState.Modified)
                        .ToList(),
                    cancellationToken);

            diagnosticsLogger
                .ChangesSaved(
                    updateEntries, 
                    entriesAffected);

            return entriesAffected;
        }

        #endregion

        #region Fields

        private readonly IGDbQueryExecutorFactory _gDbQueryExecutorFactory;

        #endregion
    }
}
