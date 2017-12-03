namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationWhere
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationWhere(
            IGDbQueryOperationArgument argument)
            : base(
                  GDbQueryOperationNames.WHERE,
                  argument,
                  2)
        {

        }

        #endregion
    }
}
