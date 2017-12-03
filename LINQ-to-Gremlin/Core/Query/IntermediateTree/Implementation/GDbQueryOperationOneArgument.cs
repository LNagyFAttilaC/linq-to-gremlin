namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public abstract class GDbQueryOperationOneArgument 
        : GDbQueryOperation
    {
        #region Constructors

        public GDbQueryOperationOneArgument(
            GDbQueryOperationNames gDbQueryOperationName,
            IGDbQueryOperationArgument argument,
            int orderLevel = -1)
            : base(
                  gDbQueryOperationName,
                  1,
                  orderLevel)
            => Argument = argument;

        #endregion

        #region Properties

        public IGDbQueryOperationArgument Argument
        {
            get
                => _arguments[0];

            private set
                => _arguments[0] = value;
        }

        #endregion
    }
}
