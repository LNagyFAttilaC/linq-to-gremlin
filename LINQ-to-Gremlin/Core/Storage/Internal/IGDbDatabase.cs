namespace LINQtoGremlin.Core.Storage.Internal
{
    public interface IGDbDatabase
    {
        bool EnsureDatabaseCreated();

        bool EnsureDatabaseDeleted();

        IGDbStore Store
        {
            get;
        }
    }
}
