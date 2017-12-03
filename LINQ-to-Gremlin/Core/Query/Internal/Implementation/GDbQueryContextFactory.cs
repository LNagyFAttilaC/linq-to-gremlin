using LINQtoGremlin.Core.Extensions;
using LINQtoGremlin.Core.Graph.LanguageSpecific;
using LINQtoGremlin.Core.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

namespace LINQtoGremlin.Core.Query.Internal
{
    public class GDbQueryContextFactory 
        : QueryContextFactory
        , IGDbQueryContextFactory
    {
        #region Constructors

        public GDbQueryContextFactory(
            QueryContextDependencies queryContextDependencies,
            IGDbStoreCache gDbStoreCache,
            IDbContextOptions dbContextOptions,
            IGDbQueryExecutorFactory gDbQueryBuilderFactory)
            : base(
                  queryContextDependencies)
            => _gDbStore
                = gDbStoreCache
                    .GetStore(
                        dbContextOptions,
                        gDbQueryBuilderFactory);

        #endregion

        #region Methods

        public override QueryContext Create()
            => new GDbQueryContext(
                Dependencies, 
                CreateQueryBuffer,
                _gDbStore);

        #endregion

        #region Fields

        private readonly IGDbStore _gDbStore;

        #endregion
    }
}
