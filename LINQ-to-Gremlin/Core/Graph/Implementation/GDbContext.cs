using LINQtoGremlin.Core.Extensions;
using LINQtoGremlin.Core.Storage.Internal;
using LINQtoGremlin.Graph.GraphElements.Vertex;
using Microsoft.EntityFrameworkCore;

namespace LINQtoGremlin.Core.Graph
{
    public class GDbContext 
        : DbContext
        , IGDbContext
    {
        #region Constructors

        public GDbContext(
            string host = "localhost",
            int port = 8182,
            bool enableSSL = false,
            string username = null,
            string password = null)
        {
            _gDbDatabaseCredentials
                = new GDbDatabaseCredentials(
                    host,
                    port,
                    enableSSL,
                    username,
                    password);
        }

        #endregion

        #region Properties

        public DbSet<Vertex> Vertices
        {
            get;
            set;
        }

        #endregion

        #region Methods

        protected override void OnConfiguring(
            DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            base
                .OnConfiguring(
                    dbContextOptionsBuilder);

            dbContextOptionsBuilder
                .UseGDatabase(
                    nameof(GDbContext),
                    _gDbDatabaseCredentials);

            dbContextOptionsBuilder
                .GetGDbContextOptionsExtension()
                .Vertex(
                    new GDbModelDescriptor(
                        typeof(Vertex))
                        .AddLabel(
                            GDbModelConstants._INNER_VERTEX_NAME,
                            false)
                        .AddProperty(
                            GDbModelConstants._INNER_APPLICATION_ID_NAME,
                            nameof(Vertex.Id)));
        }

        #endregion

        #region Fields

        protected readonly IGDbDatabaseCredentials _gDbDatabaseCredentials;

        #endregion
    }
}
