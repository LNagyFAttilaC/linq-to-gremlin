namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationMax
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationMax()
            : base(
                  GDbQueryOperationNames.MAX,
                  7)
        {

        }

        #endregion
    }
}
