using Test.TestCases.Meta;

namespace Test.TestCases
{
    public interface ITestCase
    {
        void Execute();

        void Prepare();

        void TearDown();

        TestCaseDescriptor TestCaseDescriptor
        {
            get;
        }
    }
}
