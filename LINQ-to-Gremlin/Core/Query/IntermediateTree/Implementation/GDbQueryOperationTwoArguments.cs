namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public abstract class GDbQueryOperationTwoArguments 
        : GDbQueryOperation
    {
        #region Constructors

        public GDbQueryOperationTwoArguments(
            GDbQueryOperationNames gDbQueryOperationName,
            IGDbQueryOperationArgument argument1,
            IGDbQueryOperationArgument argument2,
            int orderLevel = -1)
            : base(
                  gDbQueryOperationName,
                  2,
                  orderLevel)
        {
            Argument1 = argument1;
            Argument2 = argument2;
        }

        #endregion

        #region Properties

        public IGDbQueryOperationArgument Argument1
        {
            get
                => _arguments[0];

            private set
                => _arguments[0] = value;
        }

        public IGDbQueryOperationArgument Argument2
        {
            get
                => _arguments[1];

            private set
                => _arguments[1] = value;
        }

        #endregion
    }
}
