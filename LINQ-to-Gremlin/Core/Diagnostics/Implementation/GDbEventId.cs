using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace LINQtoGremlin.Core.Diagnostics
{
    public static class GDbEventId
    {
        #region Methods

        private static EventId MakeDatabaseTransactionId(
            Id id) 
            => new EventId(
                (int)
                    id, 
                _databaseTransactionPrefix + id);

        private static EventId MakeUpdateId(
            Id id) 
            => new EventId(
                (int)
                    id, 
                _updatePrefix + id);

        #endregion

        #region Enums

        private enum Id
        {
            DatabaseTransactionIgnoredWarning 
                = CoreEventId.ProviderBaseId + 100,
            
            ChangesSaved 
                = CoreEventId.ProviderBaseId + 200
        }

        #endregion

        #region Fields

        public static readonly EventId _changesSaved 
            = MakeUpdateId(
                Id.ChangesSaved);

        public static readonly EventId _databaseTransactionIgnoredWarning
            = MakeDatabaseTransactionId(
                Id.DatabaseTransactionIgnoredWarning);

        private static readonly string _databaseTransactionPrefix 
            = DbLoggerCategory.Database.Transaction.Name + ".";

        private static readonly string _updatePrefix 
            = DbLoggerCategory.Update.Name + ".";

        #endregion
    }
}
