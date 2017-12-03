using System;

namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationRelational
        : GDbQueryOperationTwoArguments
    {
        #region Constructors

        public GDbQueryOperationRelational(
            GDbQueryOperationNames gDbQueryOperationName,
            IGDbQueryOperationArgument argument1,
            IGDbQueryOperationArgument argument2)
            : base(
                  gDbQueryOperationName,
                  argument1,
                  argument2)
        {
            if (!(gDbQueryOperationName == GDbQueryOperationNames.RELATIONAL_EQUAL ||
                gDbQueryOperationName == GDbQueryOperationNames.RELATIONAL_GREATER_THAN_OR_EQUAL ||
                gDbQueryOperationName == GDbQueryOperationNames.RELATIONAL_GREATER_THAN ||
                gDbQueryOperationName == GDbQueryOperationNames.RELATIONAL_LESS_THAN ||
                gDbQueryOperationName == GDbQueryOperationNames.RELATIONAL_LESS_THAN_OR_EQUAL ||
                gDbQueryOperationName == GDbQueryOperationNames.RELATIONAL_NOT_EQUAL))
            {
                throw new ArgumentException(
                    "Relational operation must have relational related name.",
                    nameof(gDbQueryOperationName));
            }
        }

        #endregion
    }
}
