namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationCount
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationCount()
            : base(
                  GDbQueryOperationNames.COUNT,
                  7)
        {

        }

        #endregion
    }
}
