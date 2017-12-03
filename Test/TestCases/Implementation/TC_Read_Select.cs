using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Read_Select
        : TC_Base
    {
        #region Constructors

        public TC_Read_Select()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if 'SELECT' clause works well. Using full select, anonymous type and simple type as well.";

            _testCaseDescriptor.Name = nameof(TC_Read_Select);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    // covering: full select

                    var people1
                        = universityContext.People
                            .ToList();

                    if (people1.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + people1.Count + " != 1");
                    }

                    var person_actual
                        = people1
                            .First();

                    if (person_actual.Age != _person.Age || person_actual.Name != _person.Name || person_actual.PlaceOfBirth != _person.PlaceOfBirth)
                    {
                        throw new TestCaseFailureException(
                            "Result entity's properties are not perfect.");
                    }

                    // covering: anonymous type

                    var people2
                        = universityContext.People
                            .Select(
                                p => new 
                                    {
                                        p.Age,
                                        Name = p["Name"]
                                    })
                            .ToList();

                    if (people2.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + people2.Count + " != 1");
                    }

                    var anonymous_actual
                        = people2
                            .First();

                    if (anonymous_actual.Age != _person.Age || anonymous_actual.Name
                        .ToString() != _person.Name)
                    {
                        throw new TestCaseFailureException(
                            "Result entity's properties are not perfect.");
                    }

                    // covering: simple type

                    var ages
                        = universityContext.People
                            .Select(
                                p => p.Age)
                            .ToList();

                    if (ages.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + ages.Count + " != 1");
                    }

                    var age_actual
                        = ages
                            .First();

                    if (age_actual != _person.Age)
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
