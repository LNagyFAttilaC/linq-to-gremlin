namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationLast
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationLast(
            bool default_ = false)
            : base(
                  GDbQueryOperationNames.LAST,
                  new GDbQueryOperationConstant(
                      default_),
                  7)
        {

        }

        #endregion
    }
}
