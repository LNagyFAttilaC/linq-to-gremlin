using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Read_Where
        : TC_Base
    {
        #region Constructors

        public TC_Read_Where()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if 'WHERE' clause works well.";

            _testCaseDescriptor.Name = nameof(TC_Read_Where);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    // covering: >, <, ||

                    var people1
                        = universityContext.People
                            .Where(
                                p => p.Age > 39 || p.Age < 21)
                            .ToList();

                    if (people1.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set 1's cardinality is incorrect.\n" + people1.Count + " != 2");
                    }

                    if (people1[0].Id != _person1.Id && people1[0].Id != _person3.Id || people1[1].Id != _person1.Id && people1[1].Id != _person3.Id || people1[0].Id == people1[1].Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entities are not the expected ones.");
                    }

                    // covering: ==, &&, ||, !

                    var people2
                        = universityContext.People
                            .Where(
                                p => (p.Name == "Test Person 1" || p.Name == "Test Person 2") && !(p.PlaceOfBirth == "Test City 1"))
                            .ToList();

                    if (people2.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 2's cardinality is incorrect.\n" + people2.Count + " != 1");
                    }

                    if (people2[0].Id != _person2.Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entity is not the expected one.");
                    }

                    // covering: !=, >=, <=, &&, ^

                    var people3
                        = universityContext.People
                            .Where(
                                p => (p.PlaceOfBirth != "Test City 1" && p.Age >= 40) ^ (p.Age <= 20))
                            .ToList();

                    if (people3.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set 3's cardinality is incorrect.\n" + people3.Count + " != 2");
                    }

                    if (people3[0].Id != _person1.Id && people3[0].Id != _person3.Id || people3[1].Id != _person1.Id && people3[1].Id != _person3.Id || people3[0].Id == people3[1].Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entities are not the expected ones.");
                    }

                    // covering: parameter resolving

                    var maxAge = 35;

                    var people4
                        = universityContext.People
                            .Where(
                                p => p.Age <= maxAge)
                            .ToList();

                    if (people4.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set 4's cardinality is incorrect.\n" + people4.Count + " != 2");
                    }

                    if (people4[0].Id != _person1.Id && people4[0].Id != _person2.Id || people4[1].Id != _person1.Id && people4[1].Id != _person2.Id || people4[0].Id == people4[1].Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entities are not the expected ones.");
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
                _person1.Age = 20;
                _person1.Name = "Test Person 1";
                _person1.PlaceOfBirth = "Test City 1";

                _person2.Age = 30;
                _person2.Name = "Test Person 2";
                _person2.PlaceOfBirth = "Test City 2";

                _person3.Age = 40;
                _person3.Name = "Test Person 3";
                _person3.PlaceOfBirth = "Test City 3";

                using (var universityContext = new UniversityContext())
                {
                    universityContext.People
                        .Add(
                            _person1);

                    universityContext.People
                        .Add(
                            _person2);

                    universityContext.People
                        .Add(
                            _person3);

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

        private Person _person1 = new Person();

        private Person _person2 = new Person();

        private Person _person3 = new Person();

        #endregion
    }
}
