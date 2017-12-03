using LINQtoGremlin.Core.Graph.LanguageSpecific;
using LINQtoGremlin.Core.Graph.LanguageSpecific.GremlinQuery;
using LINQtoGremlin.Core.Infrastructure.Internal;
using LINQtoGremlin.Core.Query.ExpressionVisitors.Internal;
using LINQtoGremlin.Core.Query.Internal;
using LINQtoGremlin.Core.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace LINQtoGremlin.Core.Extensions
{
    public static class GDbExtensionsOfIServiceCollection
    {
        #region Methods

        public static IServiceCollection AddEntityFrameworkGDb(
            this IServiceCollection serviceCollection)
        {
            var builder 
                = new EntityFrameworkServicesBuilder(
                        serviceCollection)
                    .TryAdd<IDatabaseProvider, DatabaseProvider<GDbContextOptionsExtension>>()
                    .TryAdd<IDatabase, GDbDatabase>()
                    .TryAdd<IDbContextTransactionManager, GDbContextTransactionManager>()
                    .TryAdd<IDatabaseCreator, GDbDatabaseCreator>()
                    .TryAdd<IQueryContextFactory, GDbQueryContextFactory>()
                    .TryAdd<IEntityQueryModelVisitorFactory, GDbEntityQueryModelVisitorFactory>()
                    .TryAdd<IEntityQueryableExpressionVisitorFactory, GDbEntityQueryableExpressionVisitorFactory>()
                    .TryAdd<IEvaluatableExpressionFilter, EvaluatableExpressionFilter>()
                    .TryAddProviderSpecificServices(
                        b => b
                                .TryAddSingleton<IGDbStoreCache, GDbStoreCache>()
                                .TryAddScoped<IGDbQueryExecutorFactory, GDbGremlinQueryExecutorFactory>())
                    .TryAddCoreServices();

            return serviceCollection;
        }

        #endregion
    }
}
