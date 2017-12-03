namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbDatabaseCredentials 
        : IGDbDatabaseCredentials
    {
        #region Constructors

        public GDbDatabaseCredentials(
            string host,
            int port,
            bool enableSSL = false,
            string username = null,
            string password = null)
        {
            EnableSSL = enableSSL;
            Host = host;
            Password = password;
            Port = port;
            Username = username;
        }

        #endregion

        #region Properties

        public bool EnableSSL
        {
            get;
        }

        public string Host
        {
            get;
        }

        public string Password
        {
            get;
        }

        public int Port
        {
            get;
        }

        public string Username
        {
            get;
        }

        #endregion
    }
}
