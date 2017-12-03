using LINQtoGremlin.Core.Extensions;
using LINQtoGremlin.Core.Graph.LanguageSpecific;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Remotion.Linq;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbDatabase 
        : Database
        , IGDbDatabase
    {
        #region Constructors

        public GDbDatabase(
            DatabaseDependencies databaseDependencies, 
            IGDbStoreCache gDbStoreCache,
            IDbContextOptions dbContextOptions,
            IDiagnosticsLogger<DbLoggerCategory.Update> diagnosticsLogger,
            IGDbQueryExecutorFactory gDbQueryBuilderFactory) 
            : base(
                  databaseDependencies)
        {
            _gDbStore 
                = gDbStoreCache
                    .GetStore(
                        dbContextOptions,
                        gDbQueryBuilderFactory);

            _diagnosticsLogger = diagnosticsLogger;
        }

        #endregion

        #region Properties

        public virtual IGDbStore Store
            => _gDbStore;

        #endregion

        #region Methods

        public override Func<QueryContext, IEnumerable<TResult>> CompileQuery<TResult>(
            QueryModel queryModel)
        {
            while (queryModel.MainFromClause.FromExpression is SubQueryExpression)
            {
                var mainFromClauseFromExpressionQueryModel
                    = ((SubQueryExpression)
                        queryModel.MainFromClause.FromExpression).QueryModel;

                foreach (var bodyClause     in mainFromClauseFromExpressionQueryModel.BodyClauses)
                {
                    queryModel.BodyClauses
                        .Add(
                            bodyClause);
                }

                foreach (var resultOperator in mainFromClauseFromExpressionQueryModel.ResultOperators)
                {
                    queryModel.ResultOperators
                        .Add(
                            resultOperator);
                }

                queryModel.MainFromClause =    mainFromClauseFromExpressionQueryModel.MainFromClause;
            }

            return base
                .CompileQuery<TResult>(
                    queryModel);
        }

        public override int SaveChanges(
            IReadOnlyList<IUpdateEntry> updateEntries)
            => Store
                .ExecuteModifyingTransactionAsync(
                    updateEntries,
                    _diagnosticsLogger).Result;

        public override async Task<int> SaveChangesAsync(
            IReadOnlyList<IUpdateEntry> updateEntries,
            CancellationToken cancellationToken = default(CancellationToken))
            => await Store
                .ExecuteModifyingTransactionAsync(
                    updateEntries,
                    _diagnosticsLogger,
                    cancellationToken);

        public virtual bool EnsureDatabaseCreated()
            => Store
                .EnsureCreated();

        public virtual bool EnsureDatabaseDeleted()
            => Store
                .EnsureDeleted();

        #endregion

        #region Fields

        private readonly IDiagnosticsLogger<DbLoggerCategory.Update> _diagnosticsLogger;

        private readonly IGDbStore _gDbStore;

        #endregion
    }
}
