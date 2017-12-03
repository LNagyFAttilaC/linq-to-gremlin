namespace LINQtoGremlin.Core.Graph.LanguageSpecific
{
    public interface IGDbQueryExecutorFactory
    {
        IGDbQueryExecutor Create();
    }
}
