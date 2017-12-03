using LINQtoGremlin.Core.Query.IntermediateTree;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions.Internal;
using Microsoft.EntityFrameworkCore.Query.ResultOperators.Internal;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LINQtoGremlin.Core.Query.Internal
{
    public class GDbEntityQueryModelVisitor 
        : EntityQueryModelVisitor
        , IGDbEntityQueryModelVisitor
    {
        #region Constructors

        public GDbEntityQueryModelVisitor(
            EntityQueryModelVisitorDependencies entityQueryModelVisitorDependencies,
            QueryCompilationContext queryCompilationContext,
            IDiagnosticsLogger<DbLoggerCategory.Query> diagnosticsLogger)
            : base(
                  entityQueryModelVisitorDependencies,
                  queryCompilationContext)
            => _diagnosticsLogger = diagnosticsLogger;

        #endregion

        #region Properties

        public readonly MethodInfo MethodInfoOfEntityQuery
            = typeof(GDbEntityQueryModelVisitor)
                .GetTypeInfo()
                .GetDeclaredMethod(
                    nameof(EntityQuery));

        public Type ResultTypeOverride
        {
            get
                => _resultTypeOverride;
        }

        #endregion

        #region Methods

        public override void VisitMainFromClause(
            MainFromClause fromClause, 
            QueryModel queryModel)
        {
            _rootType = fromClause.ItemType;

            base
                .VisitMainFromClause(
                    fromClause, 
                    queryModel);
        }

        public override void VisitOrderByClause(
            OrderByClause orderByClause, 
            QueryModel queryModel, 
            int index)
        {
            for (var i = 0; i != orderByClause.Orderings.Count; i++)
            {
                var iThOrdering = orderByClause.Orderings[i];

                var propertyName = "";

                if (iThOrdering.Expression is MemberExpression)
                {
                    propertyName 
                        = ((MemberExpression)
                            iThOrdering.Expression).Member.Name;
                }
                else if (iThOrdering.Expression is NullConditionalExpression)
                {
                    var memberExpression
                        = (MemberExpression)
                            ((NullConditionalExpression)
                                iThOrdering.Expression).AccessOperation;

                    var additionalFromClause
                        = (AdditionalFromClause)
                            ((QuerySourceReferenceExpression)
                                memberExpression.Expression).ReferencedQuerySource;

                    propertyName
                        = additionalFromClause.ItemName
                            .Substring(
                                additionalFromClause.ItemName
                                    .IndexOf(
                                        ".") + 1) + "." + memberExpression.Member.Name;
                }

                if (i == 0)
                {
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationOrderBy(
                               new GDbQueryOperationPropertySelector(
                                   propertyName),
                                   iThOrdering.OrderingDirection
                                       .ToString() == "Desc"));
                }
                else
                {
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationThenBy(
                               new GDbQueryOperationPropertySelector(
                                   propertyName),
                                   iThOrdering.OrderingDirection
                                       .ToString() == "Desc"));
                }
            }
        }

        public override void VisitResultOperator(
            ResultOperatorBase resultOperator,
            QueryModel queryModel, 
            int index)
        {
            var resultOperatorName
                = resultOperator
                    .ToString()
                    .Substring(
                        0,
                        resultOperator
                            .ToString()
                            .IndexOf(
                                "("));

            switch (resultOperatorName)
            {
                case "Any":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationAny());
                    break;
                case "Average":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationAverage());
                    break;
                case "Count":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationCount());
                    break;
                case "First":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationFirst());
                    break;
                case "FirstOrDefault":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationFirst(
                                true));
                    break;
                case "Include":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationIncludeChain(
                                ((IncludeResultOperator)
                                    resultOperator).NavigationPropertyPaths
                                        .ToArray()));
                    break;
                case "Last":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationLast());
                    break;
                case "LastOrDefault":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationLast(
                                true));
                    break;
                case "LongCount":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationLongCount());
                    break;
                case "Max":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationMax());
                    break;
                case "Min":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationMin());
                    break;
                case "Single":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationSingle());
                    break;
                case "SingleOrDefault":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationSingle(
                                true));
                    break;
                case "Skip":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationSkip(
                                new GDbQueryOperationParameter(
                                    ((ParameterExpression)
                                        ((SkipResultOperator)
                                            resultOperator).Count).Name
                                            .ToString())));
                    break;
                case "Sum":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationSum());
                    break;
                case "Take":
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationTake(
                                new GDbQueryOperationParameter(
                                    ((ParameterExpression)
                                        ((TakeResultOperator)
                                            resultOperator).Count).Name
                                            .ToString())));
                    break;
                default:
                    if (resultOperatorName
                        .Contains("Contains") || resultOperatorName
                            .Contains("GroupBy"))
                    {
                        throw new NotSupportedException(
                            "'" + resultOperatorName + "()' method is not supported now.");
                    }
                    else
                    {
                        base
                            .VisitResultOperator(
                                resultOperator,
                                queryModel,
                                index);
                    }
                    break;
            }
        }

        public override void VisitSelectClause(
            SelectClause selectClause,
            QueryModel queryModel)
        {
            switch (selectClause.Selector.NodeType)
            {
                case ExpressionType.Extension:
                    if (selectClause.Selector is NullConditionalExpression)
                    {
                        var memberExpression
                            = (MemberExpression)
                                ((NullConditionalExpression)
                                    selectClause.Selector).AccessOperation;

                        var additionalFromClause
                            = (AdditionalFromClause)
                                ((QuerySourceReferenceExpression)
                                    memberExpression.Expression).ReferencedQuerySource;

                        _intermediateTree
                            .Add(
                                new GDbQueryOperationSelect(
                                    1)
                                    .SetArgument(
                                        0,
                                        new GDbQueryOperationPropertySelector(
                                        additionalFromClause.ItemName
                                            .Substring(
                                                additionalFromClause.ItemName
                                                    .IndexOf(
                                                        ".") + 1) + "." + memberExpression.Member.Name)));
                    }
                    break;
                case ExpressionType.MemberAccess:
                    _intermediateTree
                        .Add(
                            new GDbQueryOperationSelect(
                                1)
                                .SetArgument(
                                    0,
                                    new GDbQueryOperationPropertySelector(
                                        ((MemberExpression)
                                            selectClause.Selector).Member.Name)));
                    break;
                case ExpressionType.New:
                    var newExpression
                        = (NewExpression)
                            selectClause.Selector;

                    GDbQueryOperationSelect gDbQueryOperationSelect
                        = new GDbQueryOperationSelect(
                            newExpression.Arguments.Count);

                    for (var i = 0; i != newExpression.Arguments.Count; i++)
                    {
                        var iThArgument = newExpression.Arguments[i];

                        if (iThArgument.NodeType == ExpressionType.Convert)
                        {
                            iThArgument
                                = ((UnaryExpression)
                                    iThArgument).Operand;
                        }

                        switch (iThArgument.NodeType)
                        {
                            case ExpressionType.Call:
                                var callExpression
                                    = (MethodCallExpression)
                                        iThArgument;

                                if (callExpression.Method.Name == "get_Item")
                                {
                                    gDbQueryOperationSelect
                                        .SetArgument(
                                            i,
                                            new GDbQueryOperationPropertySelector(
                                                ((ConstantExpression)
                                                    callExpression.Arguments[0]).Value
                                                        .ToString()));
                                }
                                break;
                            case ExpressionType.Extension:
                                if (iThArgument is NullConditionalExpression)
                                {
                                    var memberExpression
                                        = (MemberExpression)
                                            ((NullConditionalExpression)
                                                iThArgument).AccessOperation;

                                    var additionalFromClause
                                        = (AdditionalFromClause)
                                            ((QuerySourceReferenceExpression)
                                                memberExpression.Expression).ReferencedQuerySource;

                                    gDbQueryOperationSelect
                                    .SetArgument(
                                        i,
                                        new GDbQueryOperationPropertySelector(
                                            additionalFromClause.ItemName
                                                .Substring(
                                                    additionalFromClause.ItemName
                                                        .IndexOf(
                                                            ".") + 1) + "." + memberExpression.Member.Name));
                                }
                                break;
                            case ExpressionType.MemberAccess:
                                gDbQueryOperationSelect
                                    .SetArgument(
                                        i,
                                        new GDbQueryOperationPropertySelector(
                                            ((MemberExpression)
                                                iThArgument).Member.Name));
                                break;
                        }
                    }

                    _intermediateTree
                        .Add(
                            gDbQueryOperationSelect);
                    break;
            }
        }

        public override void VisitQueryModel(
            QueryModel queryModel)
        {
            _resultTypeOverride = queryModel.ResultTypeOverride;

            if (_resultTypeOverride != null && _resultTypeOverride.Name
                .Contains(
                    "IIncludableQueryable"))
            {
                _resultTypeOverride = _resultTypeOverride.GenericTypeArguments[0];
            }

            base
                .VisitQueryModel(
                    queryModel);
        }

        public override void VisitWhereClause(
            WhereClause whereClause,
            QueryModel queryModel,
            int index)
        {
            _intermediateTree
                .Add(
                    new GDbQueryOperationWhere(
                        BuildGDbQueryOperationWhereArgument(
                            whereClause.Predicate)));
        }

        protected override void ExtractQueryAnnotations(
            QueryModel queryModel)
        {

        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgument(
            Expression predicate)
        {
            switch (predicate.NodeType)
            {
                case ExpressionType.Add:
                    return BuildGDbQueryOperationWhereArgumentAdd(
                        predicate);
                case ExpressionType.AndAlso:
                    return BuildGDbQueryOperationWhereArgumentAndAlso(
                        predicate);
                case ExpressionType.Constant:
                    return BuildGDbQueryOperationWhereArgumentConstant(
                        predicate);
                case ExpressionType.Call:
                    return BuildGDbQueryOperationWhereArgumentCall(
                        predicate);
                case ExpressionType.Convert:
                    return BuildGDbQueryOperationWhereArgumentConvert(
                        predicate);
                case ExpressionType.Divide:
                    return BuildGDbQueryOperationWhereArgumentDivide(
                        predicate);
                case ExpressionType.Equal:
                    return BuildGDbQueryOperationWhereArgumentEqual(
                        predicate);
                case ExpressionType.ExclusiveOr:
                    return BuildGDbQueryOperationWhereArgumentExclusiveOr(
                        predicate);
                case ExpressionType.Extension:
                    return BuildGDbQueryOperationWhereArgumentExtension(
                        predicate);
                case ExpressionType.GreaterThan:
                    return BuildGDbQueryOperationWhereArgumentGreaterThan(
                        predicate);
                case ExpressionType.GreaterThanOrEqual:
                    return BuildGDbQueryOperationWhereArgumentGreaterThanOrEqual(
                        predicate);
                case ExpressionType.LessThan:
                    return BuildGDbQueryOperationWhereArgumentLessThan(
                        predicate);
                case ExpressionType.LessThanOrEqual:
                    return BuildGDbQueryOperationWhereArgumentLessThanOrEqual(
                        predicate);
                case ExpressionType.MemberAccess:
                    return BuildGDbQueryOperationWhereArgumentPropertySelector(
                        predicate);
                case ExpressionType.Multiply:
                    return BuildGDbQueryOperationWhereArgumentMultiply(
                        predicate);
                case ExpressionType.Not:
                    return BuildGDbQueryOperationWhereArgumentNot(
                        predicate);
                case ExpressionType.NotEqual:
                    return BuildGDbQueryOperationWhereArgumentNotEqual(
                        predicate);
                case ExpressionType.OrElse:
                    return BuildGDbQueryOperationWhereArgumentOrElse(
                        predicate);
                case ExpressionType.Parameter:
                    return BuildGDbQueryOperationWhereArgumentParameter(
                        predicate);
                case ExpressionType.Subtract:
                    return BuildGDbQueryOperationWhereArgumentSubtract(
                        predicate);
                default:
                    throw new InvalidOperationException(
                        "'" + predicate.NodeType
                            .ToString()
                            .ToUpper() + "' operation is not supported in 'WHERE' clause.");
            }
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentAdd(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationArithmetic(
                GDbQueryOperationNames.ARITHMETIC_ADD,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentAndAlso(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationLogical(
                GDbQueryOperationNames.LOGICAL_AND,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentConstant(
            Expression predicate)
        {
            var constantExpression
                = (ConstantExpression)
                    predicate;

            return new GDbQueryOperationConstant(
                constantExpression.Value);
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentCall(
            Expression predicate)
        {
            var methodCallExpression
                = (MethodCallExpression)
                    predicate;

            var argument
                = methodCallExpression.Arguments[1]
                    .ToString()
                    .Replace(
                        "\"",
                        "");

            return new GDbQueryOperationPropertySelector(
                argument);
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentConvert(
            Expression predicate)
        {
            var unaryExpression
                = (UnaryExpression)
                    predicate;

            if (unaryExpression.Operand is ConstantExpression)
            {
                return new GDbQueryOperationConstant(
                    ((ConstantExpression)
                        unaryExpression.Operand).Value);
            }
            else if (unaryExpression.Operand is MethodCallExpression)
            {
                return new GDbQueryOperationConstant(
                    ((MethodCallExpression)
                        unaryExpression.Operand).Arguments[1]);
            }
            else
            {
                throw new InvalidOperationException(
                    "'" + unaryExpression.Operand.Type.Name + "' is not supported in ConvertExpression.");
            }
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentDivide(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationArithmetic(
                GDbQueryOperationNames.ARITHMETIC_DIVIDE,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentEqual(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationRelational(
                GDbQueryOperationNames.RELATIONAL_EQUAL,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentExclusiveOr(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationLogical(
                GDbQueryOperationNames.LOGICAL_EXCLUSIVE_OR,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentExtension(
            Expression predicate)
        {
            var memberExpression
                = (MemberExpression)
                    ((NullConditionalExpression)
                        predicate).AccessOperation;

            var additionalFromClause
                = (AdditionalFromClause)
                    ((QuerySourceReferenceExpression)
                        memberExpression.Expression).ReferencedQuerySource;

            return new GDbQueryOperationPropertySelector(
                additionalFromClause.ItemName
                    .Substring(
                        additionalFromClause.ItemName
                            .IndexOf(
                                ".") + 1) + "." + memberExpression.Member.Name);
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentGreaterThan(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationRelational(
                GDbQueryOperationNames.RELATIONAL_GREATER_THAN,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentGreaterThanOrEqual(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationRelational(
                GDbQueryOperationNames.RELATIONAL_GREATER_THAN_OR_EQUAL,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentLessThan(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationRelational(
                GDbQueryOperationNames.RELATIONAL_LESS_THAN,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentLessThanOrEqual(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationRelational(
                GDbQueryOperationNames.RELATIONAL_LESS_THAN_OR_EQUAL,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentMultiply(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationArithmetic(
                GDbQueryOperationNames.ARITHMETIC_MULTIPLY,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentNot(
            Expression predicate)
        {
            return new GDbQueryOperationLogical(
                GDbQueryOperationNames.LOGICAL_NOT,
                BuildGDbQueryOperationWhereArgument(
                    ((UnaryExpression)
                        predicate).Operand),
                null);
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentNotEqual(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationRelational(
                GDbQueryOperationNames.RELATIONAL_NOT_EQUAL,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentOrElse(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationLogical(
                GDbQueryOperationNames.LOGICAL_OR,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentParameter(
            Expression predicate)
        {
            return new GDbQueryOperationParameter(
                ((ParameterExpression)
                    predicate).Name);
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentPropertySelector(
            Expression predicate)
        {

            var argument
                = ((MemberExpression)
                    predicate).Member.Name
                        .Replace(
                            "\"",
                            "");

            return new GDbQueryOperationPropertySelector(
                argument);
        }

        private IGDbQueryOperationArgument BuildGDbQueryOperationWhereArgumentSubtract(
            Expression predicate)
        {
            var binaryExpression
                = (BinaryExpression)
                    predicate;

            return new GDbQueryOperationArithmetic(
                GDbQueryOperationNames.ARITHMETIC_SUBTRACT,
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Left),
                BuildGDbQueryOperationWhereArgument(
                    binaryExpression.Right));
        }
#if DEBUG
        private void DumpGDbQueryOperationArgumentsForDebug(
            IEnumerable<IGDbQueryOperationArgument> gDbQueryOperationArguments,
            int indentationLevel = 0)
        {
            foreach (var gDbQueryOperationArgument in gDbQueryOperationArguments)
            {
                if (gDbQueryOperationArgument is GDbQueryOperationConstant)
                {
                    for (var i = 0; i != indentationLevel * 2; i++)
                    {
                        Console
                            .Write(
                                " ");
                    }

                    Console
                        .WriteLine(
                            GDbQueryOperationNames.CONSTANT);

                    for (var i = 0; i != (indentationLevel + 1) * 2; i++)
                    {
                        Console
                            .Write(" ");
                    }

                    Console
                        .Write(
                            "= ");

                    Console
                        .WriteLine(
                            ((GDbQueryOperationConstant)
                                gDbQueryOperationArgument).Value);
                }
                else
                {
                    DumpGDbQueryOperationForDebug(
                        (IGDbQueryOperation)
                            gDbQueryOperationArgument,
                        indentationLevel);
                }
            }
        }
#endif
#if DEBUG
        private void DumpGDbQueryOperationForDebug(
            IGDbQueryOperation gDbQueryOperation,
            int indentationLevel = 0)
        {
            for (var i = 0; i != indentationLevel * 2; i++)
            {
                Console
                    .Write(
                        " ");
            }

            if (gDbQueryOperation != null)
            {
                Console
                    .WriteLine(
                        gDbQueryOperation
                            .GetName());

                DumpGDbQueryOperationArgumentsForDebug(
                    gDbQueryOperation
                        .GetArguments(),
                    indentationLevel + 1);
            }
            else
            {
                Console
                    .WriteLine(
                        "not set");
            }
        }
#endif
#if DEBUG
        private void DumpIntermediateTreeForDebug(
            IEnumerable<IGDbQueryOperation> intermediateTree,
            int indentationLevel = 0)
        {
            foreach (var node in intermediateTree)
            {
                DumpGDbQueryOperationForDebug(
                    node,
                    indentationLevel);
            }
        }
#endif
        private IEnumerable<TEntity> EntityQuery<TEntity>(
            QueryContext queryContext)
        {
            SubstituteParametersWithConstantValues(
                queryContext.ParameterValues,
                _intermediateTree);

            ReorderIntermediateTree();

            return ((GDbQueryContext)
                queryContext).Store
                    .ExecuteEntityQueryTransactionAsync<TEntity>(
                        _rootType,
                        _intermediateTree,
                        _diagnosticsLogger).Result;
        }

        private void ReorderIntermediateTree()
        {
            _intermediateTree 
                = _intermediateTree
                    .OrderBy(
                        o => o)
                    .ToList();
        }

        private void SubstituteParametersWithConstantValues(
            IReadOnlyDictionary<string, object> parameterValues,
            IEnumerable<IGDbQueryOperation> intermediateTree)
        {
            IList<IGDbQueryOperation> nonParameterArguments = new List<IGDbQueryOperation>();

            foreach (var node in intermediateTree)
            {
                if (node != null)
                {
                    var arguments
                        = node
                            .GetArguments();

                    for (var i = 0; i != arguments
                        .Count(); i++)
                    {
                        var argument
                            = arguments
                                .ElementAt(
                                    i);

                        if (argument is GDbQueryOperationParameter)
                        {
                            node
                                .SetArgument(
                                    i,
                                    new GDbQueryOperationConstant(
                                        parameterValues[((GDbQueryOperationParameter)
                                            argument).Value
                                                .ToString()]));
                        }
                        else if (!(argument is GDbQueryOperationConstant) && (argument is IGDbQueryOperation))
                        {
                            nonParameterArguments
                                .Add(
                                    (IGDbQueryOperation)
                                        argument);
                        }
                    }
                }
            }

            if (nonParameterArguments.Count > 0)
            {
                SubstituteParametersWithConstantValues(
                    parameterValues,
                    nonParameterArguments);
            }
        }

        #endregion

        #region Fields

        private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _diagnosticsLogger;

        private IList<IGDbQueryOperation> _intermediateTree = new List<IGDbQueryOperation>();

        private Type _resultTypeOverride;

        private Type _rootType;

        #endregion
    }
}
