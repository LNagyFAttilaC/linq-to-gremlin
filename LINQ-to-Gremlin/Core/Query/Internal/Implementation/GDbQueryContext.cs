using LINQtoGremlin.Core.Storage.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;

namespace LINQtoGremlin.Core.Query.Internal
{
    public class GDbQueryContext 
        : QueryContext
        , IGDbQueryContext
    {
        #region Constructors

        public GDbQueryContext(
            QueryContextDependencies dependencies,
            Func<IQueryBuffer> queryBufferFactory,
            IGDbStore gDbStore)
            : base(
                  dependencies,
                  queryBufferFactory)
            => Store = gDbStore;

        #endregion

        #region Properties

        public virtual IGDbStore Store
        {
            get;
        }

        #endregion
    }
}
