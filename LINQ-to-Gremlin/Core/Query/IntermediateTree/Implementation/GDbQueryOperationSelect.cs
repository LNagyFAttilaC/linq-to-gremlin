namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationSelect
        : GDbQueryOperation
    {
        #region Constructors

        public GDbQueryOperationSelect(
            int numberOfArguments)
            : base(
                  GDbQueryOperationNames.SELECT,
                  numberOfArguments,
                  6)
        {

        }

        #endregion
    }
}
