using LINQtoGremlin.Core.Extensions.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbContextTransactionManager 
        : IDbContextTransactionManager
        , IGDbContextTransactionManager
    {
        #region Constructors

        public GDbContextTransactionManager(
            IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> diagnosticsLogger)
            => _diagnosticsLogger = diagnosticsLogger;

        #endregion

        #region Properties

        public virtual IDbContextTransaction CurrentTransaction 
            => null;

        #endregion

        #region Methods

        public virtual IDbContextTransaction BeginTransaction()
        {
            _diagnosticsLogger
                .DatabaseTransactionIgnoredWarning();

            return _gDbContextTransaction;
        }

        public virtual Task<IDbContextTransaction> BeginTransactionAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            _diagnosticsLogger
                .DatabaseTransactionIgnoredWarning();

            return Task
                .FromResult<IDbContextTransaction>(
                    _gDbContextTransaction);
        }

        public virtual void CommitTransaction()
            => _diagnosticsLogger
                .DatabaseTransactionIgnoredWarning();

        public virtual void ResetState()
        {

        }

        public virtual void RollbackTransaction()
            => _diagnosticsLogger
                .DatabaseTransactionIgnoredWarning();

        #endregion

        #region Fields

        private readonly IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> _diagnosticsLogger;

        private static readonly GDbContextTransaction _gDbContextTransaction = new GDbContextTransaction();

        #endregion
    }
}
