namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationThenBy
        : GDbQueryOperationTwoArguments
    {
        #region Constructors

        public GDbQueryOperationThenBy(
            GDbQueryOperationPropertySelector gDbQueryOperationPropertySelector,
            bool descending = false)
            : base(
                  GDbQueryOperationNames.THEN_BY,
                  gDbQueryOperationPropertySelector,
                  new GDbQueryOperationConstant(
                      descending),
                  4)
        {

        }

        #endregion
    }
}
