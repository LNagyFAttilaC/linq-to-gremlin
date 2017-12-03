using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Read_ResultOperators
        : TC_Base
    {
        #region Constructors

        public TC_Read_ResultOperators()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if result operators works well.";

            _testCaseDescriptor.Name = nameof(TC_Read_ResultOperators);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    // covering: any

                    var any1
                        = universityContext.People
                            .Any(
                                p => p.Age <= 20);

                    var any2
                        = universityContext.People
                            .Any(
                                p => p.Age <= 10);

                    if (any1 != true || any2 != false)
                    {
                        throw new TestCaseFailureException(
                            "'ANY' is incorrect.");
                    }

                    // covering: average

                    var average
                        = universityContext.People
                            .Average(
                                p => p.Age);

                    if (average != (_person1.Age + _person2.Age + _person3.Age) / 3.0)
                    {
                        throw new TestCaseFailureException(
                            "'AVERAGE' is incorrect.");
                    }

                    // covering: count

                    var count
                        = universityContext.People
                            .Count();

                    if (count != 3)
                    {
                        throw new TestCaseFailureException(
                            "'COUNT' is incorrect.");
                    }

                    // covering: first

                    var first
                        = universityContext.People
                            .OrderBy(
                                p => p.Age)
                            .First();

                    if (first.Id != _person1.Id)
                    {
                        throw new TestCaseFailureException(
                            "'FIRST' is incorrect.");
                    }

                    // covering: first or default

                    var firstordefault
                        = universityContext.People
                            .Where(
                                p => p.Age == 2000)
                            .FirstOrDefault();

                    if (firstordefault != null)
                    {
                        throw new TestCaseFailureException(
                            "'FIRST OR DEFAULT' is incorrect.");
                    }

                    // covering: last

                    var last
                        = universityContext.People
                            .OrderBy(
                                p => p.Age)
                            .Last();

                    if (last.Id != _person3.Id)
                    {
                        throw new TestCaseFailureException(
                            "'LAST' is incorrect.");
                    }

                    // covering: last or default

                    var lastordefault
                        = universityContext.People
                            .Where(
                                p => p.Age == 2000)
                            .FirstOrDefault();

                    if (lastordefault != null)
                    {
                        throw new TestCaseFailureException(
                            "'LAST OR DEFAULT' is incorrect.");
                    }

                    // covering: long count

                    var longcount
                        = universityContext.People
                            .LongCount();

                    if (longcount != 3L)
                    {
                        throw new TestCaseFailureException(
                            "'LONG COUNT' is incorrect.");
                    }

                    // covering: max

                    var max
                        = universityContext.People
                            .Max(
                                p => p.Age);

                    if (max != 40)
                    {
                        throw new TestCaseFailureException(
                            "'MAX' is incorrect.");
                    }

                    // covering: min

                    var min
                        = universityContext.People
                            .Min(
                                p => p.Age);

                    if (min != 20)
                    {
                        throw new TestCaseFailureException(
                            "'MIN' is incorrect.");
                    }

                    // covering: single

                    var single
                        = universityContext.People
                            .Where(
                                p => p.Age == 30)
                            .Single();

                    if (single.Id != _person2.Id)
                    {
                        throw new TestCaseFailureException(
                            "'SINGLE' is incorrect.");
                    }

                    // covering: single or default

                    var singleordefault
                        = universityContext.People
                            .Where(
                                p => p.Age == 50)
                            .SingleOrDefault();

                    if (singleordefault != null)
                    {
                        throw new TestCaseFailureException(
                            "'SINGLE OR DEFAULT' is incorrect.");
                    }

                    // covering: skip

                    // commented out, because skip method is not currently supported by Gremlin-Server

                    // var skip
                    //     = universityContext.People
                    //         .OrderBy(
                    //             p => p.Age)
                    //         .Skip(
                    //             1)
                    //         .ToList();

                    // if (skip.Count != 2)
                    // {
                    //     throw new TestCaseFailureException(
                    //         "'SKIP' is incorrect.");
                    // }

                    // covering: sum

                    var sum
                        = universityContext.People
                            .Sum(
                                p => p.Age);

                    if (sum != 90)
                    {
                        throw new TestCaseFailureException(
                            "'SUM' is incorrect.");
                    }

                    // covering: take

                    var take
                        = universityContext.People
                            .OrderBy(
                                p => p.Age)
                            .Take(
                                2)
                            .ToList();

                    if (take.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "'TAKE' is incorrect.");
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
