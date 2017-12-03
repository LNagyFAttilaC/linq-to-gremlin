namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationSum
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationSum()
            : base(
                  GDbQueryOperationNames.SUM,
                  7)
        {

        }

        #endregion
    }
}
