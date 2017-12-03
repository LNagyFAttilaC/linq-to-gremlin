using System;
using System.Collections.Generic;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Create_Edges_Multiple
        : TC_Base
    {
        #region Constructors

        public TC_Create_Edges_Multiple()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entity edges (relationships) are added well. Using multiple relationship.";

            _testCaseDescriptor.Name = nameof(TC_Create_Edges_Multiple);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    var subjects
                        = universityContext.Subjects
                            .ToList();

                    if (subjects.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + subjects.Count + " != 2");
                    }

                    if (subjects[0].Id != _subject1.Id && subjects[0].Id != _subject2.Id || subjects[1].Id != _subject1.Id && subjects[1].Id != _subject2.Id || subjects[0].Id == subjects[1].Id)
                    {
                        throw new TestCaseFailureException(
                            "Edges were not added perfectly.");
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
                _department.FoundedIn = 2000;
                _department.Name = "Test Department 1";
                _department.Subjects = new List<Subject>();
                _department.Subjects
                    .Add(
                        _subject1);
                _department.Subjects
                    .Add(
                        _subject2);

                _subject1.Name = "Test Subject 1";

                _subject2.Name = "Test Subject 2";

                using (var universityContext = new UniversityContext())
                {
                    universityContext.Departments
                        .Add(
                            _department);

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

        private Department _department = new Department();

        private Subject _subject1 = new Subject();

        private Subject _subject2 = new Subject();

        #endregion
    }
}
