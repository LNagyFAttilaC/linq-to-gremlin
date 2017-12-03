using System;
using System.Collections.Generic;

namespace LINQtoGremlin.Core.Query.IntermediateTree
{
    public abstract class GDbQueryOperation
        : IGDbQueryOperation
    {
        #region Constructors

        public GDbQueryOperation(
            GDbQueryOperationNames gDbQueryOperationName,
            int numberOfArguments,
            int orderLevel = -1)
        {
            _arguments = new IGDbQueryOperationArgument[numberOfArguments];
            _gDbQueryOperationName = gDbQueryOperationName;
            _orderLevel = orderLevel;
        }

        #endregion

        #region Properties

        public object Value
        {
            get
                => this;
        }

        #endregion

        #region Methods

        public int CompareTo(
            object other)
        {
            if (other == null)
            {
                return 1;
            }

            if (other is GDbQueryOperation)
            {
                return _orderLevel - ((GDbQueryOperation)
                    other)._orderLevel;
            }
            else
            {
                throw new ArgumentException(
                    "Should be compared only with an other " + nameof(GDbQueryOperation) + ".",
                    nameof(other));
            }
        }

        public IEnumerable<IGDbQueryOperationArgument> GetArguments()
            => _arguments;

        public GDbQueryOperationNames GetName()
            => _gDbQueryOperationName;

        public IGDbQueryOperation SetArgument(
            int index,
            IGDbQueryOperationArgument argument)
        {
            _arguments[index] = argument;

            return this;
        }

        #endregion

        #region Fields

        protected readonly IGDbQueryOperationArgument[] _arguments;

        protected readonly GDbQueryOperationNames _gDbQueryOperationName;

        private readonly int _orderLevel;

        #endregion
    }
}
