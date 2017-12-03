using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbDatabaseCreator 
        : IDatabaseCreator
        , IGDbDatabaseCreator
    {
        #region Constructors

        public GDbDatabaseCreator(
            IGDbDatabase database)
            => _gDatabase = database;

        #endregion

        #region Methods

        public virtual bool EnsureCreated()
            => _gDatabase
                .EnsureDatabaseCreated();

        public virtual Task<bool> EnsureCreatedAsync(
            CancellationToken cancellationToken = default(CancellationToken))
            => Task
                .FromResult(
                    EnsureCreated());

        public virtual bool EnsureDeleted()
            => _gDatabase
                .EnsureDatabaseDeleted();

        public virtual Task<bool> EnsureDeletedAsync(
            CancellationToken cancellationToken = default(CancellationToken))
            => Task
                .FromResult(
                    EnsureDeleted());

        #endregion

        #region Fields

        private readonly IGDbDatabase _gDatabase;

        #endregion
    }
}
