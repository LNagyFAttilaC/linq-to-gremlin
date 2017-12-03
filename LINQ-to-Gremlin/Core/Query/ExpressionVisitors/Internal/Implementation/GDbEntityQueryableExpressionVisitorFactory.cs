using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;
using System.Linq.Expressions;

namespace LINQtoGremlin.Core.Query.ExpressionVisitors.Internal
{
    public class GDbEntityQueryableExpressionVisitorFactory 
        : IEntityQueryableExpressionVisitorFactory
        , IGDbEntityQueryableExpressionVisitorFactory
    {
        #region Methods

        public virtual ExpressionVisitor Create(
            EntityQueryModelVisitor entityQueryModelVisitor,
            IQuerySource querySource)
            => new GDbEntityQueryableExpressionVisitor(
                entityQueryModelVisitor,
                querySource);

        #endregion
    }
}
