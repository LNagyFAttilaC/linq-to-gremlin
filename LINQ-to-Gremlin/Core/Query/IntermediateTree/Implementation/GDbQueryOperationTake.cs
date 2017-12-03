namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationTake
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationTake(
            int number)
            : base(
                  GDbQueryOperationNames.TAKE,
                  new GDbQueryOperationConstant(
                      number),
                  7)
        {

        }

        public GDbQueryOperationTake(
            GDbQueryOperationParameter gDbQueryOperationParameter)
            : base(
                  GDbQueryOperationNames.TAKE,
                  gDbQueryOperationParameter,
                  7)
        {

        }

        #endregion
    }
}
