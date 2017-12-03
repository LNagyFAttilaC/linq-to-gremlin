using LINQtoGremlin.Core.Graph.LanguageSpecific;
using LINQtoGremlin.Core.Infrastructure.Internal;
using LINQtoGremlin.Core.Storage.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;

namespace LINQtoGremlin.Core.Extensions
{
    public static class GDbExtensionsOfIGDbStoreCache
    {
        #region Methods

        public static IGDbStore GetStore(
            this IGDbStoreCache gStoreCache,
            IDbContextOptions dbContextOptions, 
            IGDbQueryExecutorFactory gDbQueryBuilderFactory)
            => gStoreCache
                .GetStore(
                    dbContextOptions.Extensions
                        .OfType<GDbContextOptionsExtension>()
                        .First()
                        .StoreName, 
                    gDbQueryBuilderFactory);

        #endregion
    }
}
