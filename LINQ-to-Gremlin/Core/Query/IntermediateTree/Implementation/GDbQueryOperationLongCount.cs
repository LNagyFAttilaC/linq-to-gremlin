namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationLongCount
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationLongCount()
            : base(
                  GDbQueryOperationNames.LONG_COUNT,
                  7)
        {

        }

        #endregion
    }
}
