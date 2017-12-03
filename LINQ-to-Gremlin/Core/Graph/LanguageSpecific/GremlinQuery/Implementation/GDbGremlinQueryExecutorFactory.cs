using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LINQtoGremlin.Core.Graph.LanguageSpecific.GremlinQuery
{
    public class GDbGremlinQueryExecutorFactory 
        : IGDbQueryExecutorFactory
        , IGDbGremlinQueryExecutorFactory
    {
        #region Constructors

        public GDbGremlinQueryExecutorFactory(
            IDbContextOptions dbContextOptions)
            => _dbContextOptions = dbContextOptions;

        #endregion

        #region Methods

        public IGDbQueryExecutor Create()
            => new GDbGremlinQueryExecutor(
                _dbContextOptions);

        #endregion

        #region Fields

        private readonly IDbContextOptions _dbContextOptions;

        #endregion
    }
}
