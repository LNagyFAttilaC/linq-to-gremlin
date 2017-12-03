namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationPropertySelector
        : GDbQueryOperationOneArgument
    {
        #region Constructors

        public GDbQueryOperationPropertySelector(
            string name)
            : base(
                  GDbQueryOperationNames.PROPERTY_SELECT,
                  new GDbQueryOperationConstant(
                      name))
        {

        }

        #endregion
    }
}
