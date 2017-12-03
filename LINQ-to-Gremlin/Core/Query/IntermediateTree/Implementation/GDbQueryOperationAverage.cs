namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationAverage
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationAverage()
            : base(
                  GDbQueryOperationNames.AVERAGE,
                  7)
        {

        }

        #endregion
    }
}
