namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public class GDbQueryOperationIncludeChain
        : GDbQueryOperation
    {
        #region Constructors

        public GDbQueryOperationIncludeChain(
            params string[] properties)
            : base(
                  GDbQueryOperationNames.INCLUDE_CHAIN,
                  properties.Length,
                  1)
        {
            for (var i = 0; i != properties.Length; i++)
            {
                SetArgument(
                    i,
                    new GDbQueryOperationPropertySelector(
                        properties[i]));
            }
        }

        #endregion
    }
}
