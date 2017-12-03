using System;

namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationLogical
        : GDbQueryOperationTwoArguments
    {
        #region Constructors

        public GDbQueryOperationLogical(
            GDbQueryOperationNames gDbQueryOperationName,
            IGDbQueryOperationArgument argument1,
            IGDbQueryOperationArgument argument2)
            : base(
                  gDbQueryOperationName,
                  argument1,
                  argument2)
        {
            if (!(gDbQueryOperationName == GDbQueryOperationNames.LOGICAL_AND ||
                gDbQueryOperationName == GDbQueryOperationNames.LOGICAL_EXCLUSIVE_OR ||
                gDbQueryOperationName == GDbQueryOperationNames.LOGICAL_NOT ||
                gDbQueryOperationName == GDbQueryOperationNames.LOGICAL_OR))
            {
                throw new ArgumentException(
                    "Logical operation must have logical related name.",
                    nameof(gDbQueryOperationName));
            }
        }

        #endregion
    }
}
