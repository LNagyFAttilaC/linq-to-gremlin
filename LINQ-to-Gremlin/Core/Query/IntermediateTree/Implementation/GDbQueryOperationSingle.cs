namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationSingle
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationSingle(
            bool default_ = false)
            : base(
                  GDbQueryOperationNames.SINGLE,
                  new GDbQueryOperationConstant(
                      default_),
                  7)
        {

        }

        #endregion
    }
}
