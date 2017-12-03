using System;

namespace LINQtoGremlin.Core.Diagnostics.Exceptions
{
    public class GDbDatabaseIntegrityDamagedException 
        : ApplicationException
        , IGDbDatabaseIntegrityDamagedException
    {
        #region Constructors

        public GDbDatabaseIntegrityDamagedException(
            string message)
            : base(
                  message)
        {

        }

        #endregion
    }
}
