using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Read_AllOfAType
        : TC_Base
    {
        #region Constructors

        public TC_Read_AllOfAType()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if all entity of a type can be read. Using inheritance, too.";

            _testCaseDescriptor.Name = nameof(TC_Read_AllOfAType);
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

                    if (people.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + people.Count + " != 2");
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

                _teacher.Age = 30;
                _teacher.Name = "Test Teacher 1";
                _teacher.PlaceOfBirth = "Test City 2";
                _teacher.TeacherCode = "Test Teacher Code 1";

                using (var universityContext = new UniversityContext())
                {
                    universityContext.People
                        .Add(
                            _person);

                    universityContext.People
                        .Add(
                            _teacher);

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

        private Teacher _teacher = new Teacher();

        #endregion
    }
}
