using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Read_Ordering
        : TC_Base
    {
        #region Constructors

        public TC_Read_Ordering()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if ordering works well.";

            _testCaseDescriptor.Name = nameof(TC_Read_Ordering);
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
                            .OrderBy(
                                p => p.Age)
                            .ThenByDescending(
                                p => p.Name)
                            .ToList();

                    if (people1.Count != 4)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + people1.Count + " != 4");
                    }

                    if (people1[0].Id != _person1.Id || people1[1].Id != _person4.Id || people1[2].Id != _person3.Id || people1[3].Id != _person2.Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entites' order is not perfect.");
                    }

                    var people2
                        = universityContext.People
                            .OrderByDescending(
                                p => p.Age)
                            .ThenBy(
                                p => p.Name)
                            .ToList();

                    if (people2.Count != 4)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + people2.Count + " != 4");
                    }

                    if (people2[0].Id != _person2.Id || people2[1].Id != _person3.Id || people2[2].Id != _person4.Id || people2[3].Id != _person1.Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entites' order is not perfect.");
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

                _person3.Age = 30;
                _person3.Name = "Test Person 3";
                _person3.PlaceOfBirth = "Test City 2";

                _person4.Age = 25;
                _person4.Name = "Test Person 4";
                _person4.PlaceOfBirth = "Test City 3";

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

                    universityContext.People
                        .Add(
                            _person4);

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

        private Person _person4 = new Person();

        #endregion
    }
}
