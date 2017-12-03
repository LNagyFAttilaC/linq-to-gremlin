namespace LINQtoGremlin.Core.Storage.Internal
{
    public interface IGDbDatabaseCredentials
    {
        bool EnableSSL
        {
            get;
        }

        string Host
        {
            get;
        }

        string Password
        {
            get;
        }

        int Port
        {
            get;
        }

        string Username
        {
            get;
        }
    }
}
