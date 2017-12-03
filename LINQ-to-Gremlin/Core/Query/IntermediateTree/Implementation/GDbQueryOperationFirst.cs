namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationFirst
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationFirst(
            bool default_ = false)
            : base(
                  GDbQueryOperationNames.FIRST,
                  new GDbQueryOperationConstant(
                      default_),
                  7)
        {

        }

        #endregion
    }
}
