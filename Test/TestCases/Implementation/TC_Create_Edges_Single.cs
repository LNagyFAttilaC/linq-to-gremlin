using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Create_Edges_Single
        : TC_Base
    {
        #region Constructors

        public TC_Create_Edges_Single()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entity edges (relationships) are added well. Using single relationship.";

            _testCaseDescriptor.Name = nameof(TC_Create_Edges_Single);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    var teachers
                        = universityContext.Teachers
                            .ToList();

                    if (teachers.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set cardinality is incorrect.\n" + teachers.Count + " != 1");
                    }

                    var teacher_actual
                        = teachers
                            .First();

                    if (teacher_actual.Id != _teacher.Id)
                    {
                        throw new TestCaseFailureException(
                            "Edge was not added perfectly.");
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
                _teacher.Age = 20;
                _teacher.Name = "Test Teacher 1";
                _teacher.PlaceOfBirth = "Test City 1";
                _teacher.TeacherCode = "Test Teacher Code 1";

                _university.FoundedIn = 2000;
                _university.Name = "Test University 1";
                _university.Rector = _teacher;

                using (var universityContext = new UniversityContext())
                {
                    universityContext.Universities
                        .Add(
                            _university);

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

        private Teacher _teacher = new Teacher();

        private University _university = new University();

        #endregion
    }
}
