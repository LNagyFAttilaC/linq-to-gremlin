namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationOrderBy
        : GDbQueryOperationTwoArguments
    {
        #region Constructors

        public GDbQueryOperationOrderBy(
            GDbQueryOperationPropertySelector gDbQueryOperationPropertySelector,
            bool descending = false)
            : base(
                  GDbQueryOperationNames.ORDER_BY,
                  gDbQueryOperationPropertySelector,
                  new GDbQueryOperationConstant(
                      descending),
                  3)
        {

        }

        #endregion
    }
}
