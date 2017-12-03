namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationSkip
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationSkip(
            int number)
            : base(
                  GDbQueryOperationNames.SKIP,
                  new GDbQueryOperationConstant(
                      number),
                  7)
        {

        }

        public GDbQueryOperationSkip(
           GDbQueryOperationParameter gDbQueryOperationParameter)
            : base(
                  GDbQueryOperationNames.SKIP,
                  gDbQueryOperationParameter,
                  7)
        {

        }

        #endregion
    }
}
