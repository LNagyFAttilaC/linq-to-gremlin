using System;

namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationArithmetic
        : GDbQueryOperationTwoArguments
    {
        #region Constructors

        public GDbQueryOperationArithmetic(
            GDbQueryOperationNames gDbQueryOperationName,
            IGDbQueryOperationArgument argument1,
            IGDbQueryOperationArgument argument2)
            : base(
                  gDbQueryOperationName,
                  argument1,
                  argument2)
        {
            if (!(gDbQueryOperationName == GDbQueryOperationNames.ARITHMETIC_ADD ||
                gDbQueryOperationName == GDbQueryOperationNames.ARITHMETIC_DIVIDE ||
                gDbQueryOperationName == GDbQueryOperationNames.ARITHMETIC_MULTIPLY ||
                gDbQueryOperationName == GDbQueryOperationNames.ARITHMETIC_SUBTRACT))
            {
                throw new ArgumentException(
                    "Arithmetic operation must have arithmetic related name.",
                    nameof(gDbQueryOperationName));
            }
        }

        #endregion
    }
}
