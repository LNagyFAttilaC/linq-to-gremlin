using System;
using System.Collections.Generic;

namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public interface IGDbQueryOperation
        : IComparable
        , IGDbQueryOperationArgument
    {
        IEnumerable<IGDbQueryOperationArgument> GetArguments();

        GDbQueryOperationNames GetName();

        IGDbQueryOperation SetArgument(
            int index,
            IGDbQueryOperationArgument argument);
    }
}
