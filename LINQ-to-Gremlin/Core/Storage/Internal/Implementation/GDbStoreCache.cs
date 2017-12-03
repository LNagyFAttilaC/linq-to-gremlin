using LINQtoGremlin.Core.Graph.LanguageSpecific;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbStoreCache 
        : IGDbStoreCache
    {
        #region Methods

        public IGDbStore GetStore(
            string storeName,
            IGDbQueryExecutorFactory gDbQueryBuilderFactory)
            => _namedStores.Value
                .GetOrAdd(
                    storeName,
                    s => new GDbStore(
                        gDbQueryBuilderFactory));

        #endregion

        #region Fields

        private readonly Lazy<ConcurrentDictionary<string, IGDbStore>> _namedStores
            = new Lazy<ConcurrentDictionary<string, IGDbStore>>(
                () => new ConcurrentDictionary<string, IGDbStore>(), 
                LazyThreadSafetyMode.PublicationOnly);

        #endregion
    }
}
