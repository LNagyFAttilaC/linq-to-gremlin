using LINQtoGremlin.Core.Extensions.Internal;
using LINQtoGremlin.Core.Query.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Remotion.Linq.Clauses;
using System;
using System.Linq.Expressions;

namespace LINQtoGremlin.Core.Query.ExpressionVisitors.Internal
{
    public class GDbEntityQueryableExpressionVisitor 
        : EntityQueryableExpressionVisitor
        , IGDbEntityQueryableExpressionVisitor
    {
        #region Constructors

        public GDbEntityQueryableExpressionVisitor(
            EntityQueryModelVisitor entityQueryModelVisitor,
            IQuerySource querySource)
            : base(
                  entityQueryModelVisitor)
            => _querySource = querySource;

        #endregion

        #region Properties

        private new GDbEntityQueryModelVisitor QueryModelVisitor
            => (GDbEntityQueryModelVisitor)
                base.QueryModelVisitor;

        #endregion

        #region Methods

        protected override Expression VisitEntityQueryable(
            Type type)
            => Expression
                .Call(
                    Expression
                        .Constant(
                            QueryModelVisitor),
                    QueryModelVisitor.MethodInfoOfEntityQuery
                        .MakeGenericMethod(
                            QueryModelVisitor.ResultTypeOverride
                                .IsCollectionType()
                                ? new Type[]
                                    {
                                        QueryModelVisitor.ResultTypeOverride.GenericTypeArguments[0]
                                    }
                                : new Type[] 
                                    {
                                        QueryModelVisitor.ResultTypeOverride
                                    }),
                    EntityQueryModelVisitor.QueryContextParameter);

        #endregion

        #region Fields

        private readonly IQuerySource _querySource;

        #endregion
    }
}
