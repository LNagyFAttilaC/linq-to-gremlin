using LINQtoGremlin.Core.Graph.LanguageSpecific;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public interface IGDbStoreCache
    {
        IGDbStore GetStore(
            string storeName,
            IGDbQueryExecutorFactory gDbQueryBuilderFactory);
    }
}
