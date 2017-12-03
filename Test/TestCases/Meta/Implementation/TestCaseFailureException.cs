using System;

namespace Test.TestCases.Meta
{
    public class TestCaseFailureException
        : Exception
    {
        #region Constructors

        public TestCaseFailureException(
            string message)
            : base(
                  message)
        {

        }

        #endregion
    }
}
