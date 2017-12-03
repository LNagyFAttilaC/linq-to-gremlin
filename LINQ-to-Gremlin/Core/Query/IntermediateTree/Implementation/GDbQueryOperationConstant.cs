namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationConstant
        : IGDbQueryOperationArgument
    {
        #region Constructors

        public GDbQueryOperationConstant(
            object value)
            => Value = value;

        #endregion

        #region Properties

        public object Value
        {
            get;
        }

        #endregion
    }
}
