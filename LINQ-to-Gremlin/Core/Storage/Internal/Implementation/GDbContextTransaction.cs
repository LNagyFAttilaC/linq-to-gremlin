using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace LINQtoGremlin.Core.Storage.Internal
{
    public class GDbContextTransaction 
        : IDbContextTransaction
        , IGDbContextTransaction
    {
        #region Properties

        public virtual Guid TransactionId
        {
            get;
        }
            = Guid
                .NewGuid();

        #endregion

        #region Methods

        public virtual void Commit()
        {
            
        }

        public virtual void Dispose()
        {
            
        }

        public virtual void Rollback()
        {
            
        }

        #endregion
    }
}
