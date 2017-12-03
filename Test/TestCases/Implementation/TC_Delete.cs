using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Delete
        : TC_Base
    {
        #region Constructors

        public TC_Delete()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entities are deleted well.";

            _testCaseDescriptor.Name = nameof(TC_Delete);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                TearDown();

                using (var universityContext = new UniversityContext())
                {
                    var count
                        = universityContext.Vertices
                            .Count();

                    if (count != 0)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + count + " != 0");
                    }
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

        public override void Prepare()
        {
            try
            {
                Person person = new Person();

                person.Age = 20;
                person.Name = "Test Person 1";
                person.PlaceOfBirth = "Test City 1";

                using (var universityContext = new UniversityContext())
                {
                    universityContext.People
                        .Add(
                            person);

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

            base
                .Prepare();
        }

        #endregion
    }
}
