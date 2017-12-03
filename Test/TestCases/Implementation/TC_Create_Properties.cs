using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Create_Properties
        : TC_Base
    {
        #region Constructors

        public TC_Create_Properties()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entity properties are added well.";

            _testCaseDescriptor.Name = nameof(TC_Create_Properties);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    var people
                        = universityContext.People
                            .ToList();

                    if (people.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + people.Count + " != 1");
                    }

                    var person_actual
                        = people
                            .First();

                    if (person_actual.Age != _person.Age || person_actual.Name != _person.Name || person_actual.PlaceOfBirth != _person.PlaceOfBirth)
                    {
                        throw new TestCaseFailureException(
                            "Result entity's properties are not perfect.");
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
                _person.Age = 20;
                _person.Name = "Test Person 1";
                _person.PlaceOfBirth = "Test City 1";

                using (var universityContext = new UniversityContext())
                {
                    universityContext.People
                        .Add(
                            _person);

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

        #region Fields

        private Person _person = new Person();

        #endregion
    }
}
