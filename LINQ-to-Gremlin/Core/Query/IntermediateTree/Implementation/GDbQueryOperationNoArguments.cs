namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public abstract class GDbQueryOperationNoArguments
        : GDbQueryOperation
    {
        #region Constructors

        public GDbQueryOperationNoArguments(
            GDbQueryOperationNames gDbQueryOperationName,
            int orderLevel = -1)
            : base(
                  gDbQueryOperationName,
                  0,
                  orderLevel)
        {

        }

        #endregion
    }
}
