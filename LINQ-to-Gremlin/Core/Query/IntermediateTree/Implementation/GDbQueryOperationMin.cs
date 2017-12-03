namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationMin
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationMin()
            : base(
                  GDbQueryOperationNames.MIN,
                  7)
        {

        }

        #endregion
    }
}
