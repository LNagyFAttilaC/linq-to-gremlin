using System;
using System.Linq;
using Test.Context;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public abstract class TC_Base
        : ITestCase
    {
        #region Properties

        public TestCaseDescriptor TestCaseDescriptor
            => _testCaseDescriptor;

        #endregion

        #region Methods

        public abstract void Execute();

        public virtual void Prepare()
        {

        }

        public virtual void TearDown()
        {
            try
            {
                using (UniversityContext universityContext = new UniversityContext())
                {
                    var vertices
                        = universityContext.Vertices
                            .ToList();

                    foreach (var vertex in vertices)
                    {
                        universityContext.Vertices
                            .Remove(
                                vertex);
                    }

                    universityContext
                        .SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new TestCaseFailureException(
                    "Exception catched:\n" + e.Message + ((e.InnerException != null)
                        ? "\n\tInner exception:\n\t\t" + e.InnerException.Message
                        : ""));
            }
        }


        #endregion

        #region Fields

        protected readonly TestCaseDescriptor _testCaseDescriptor = new TestCaseDescriptor();

        #endregion
    }
}
