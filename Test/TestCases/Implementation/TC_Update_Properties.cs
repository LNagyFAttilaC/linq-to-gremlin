using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Update_Properties
        : TC_Base
    {
        #region Constructors

        public TC_Update_Properties()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entity properties are modified well.";

            _testCaseDescriptor.Name = nameof(TC_Update_Properties);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    var people1
                        = universityContext.People
                            .ToList();

                    if (people1.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 1's cardinality is incorrect.\n" + people1.Count + " != 1");
                    }

                    var person_actual1
                        = people1
                            .First();

                    person_actual1.Age = 30;
                    person_actual1.Name = "Test Person 1 2";
                    person_actual1.PlaceOfBirth = "Test City 1 2";

                    universityContext
                        .Update(
                            person_actual1);

                    universityContext
                        .SaveChanges();

                    var people2
                        = universityContext.People
                            .ToList();

                    if (people2.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 2's cardinality is incorrect.\n" + people2.Count + " != 1");
                    }

                    var person_actual2
                        = people2
                            .First();

                    if (person_actual2.Age != person_actual1.Age || person_actual2.Name != person_actual1.Name || person_actual2.PlaceOfBirth != person_actual1.PlaceOfBirth)
                    {
                        throw new TestCaseFailureException(
                            "Modified result entity's properties are not perfect.");
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
