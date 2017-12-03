namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationAny
        : GDbQueryOperationNoArguments
    {
        #region Constructors

        public GDbQueryOperationAny()
            : base(
                  GDbQueryOperationNames.ANY,
                  7)
        {

        }

        #endregion
    }
}
