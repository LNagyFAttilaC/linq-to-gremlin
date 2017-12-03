using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Create_Inheritance
        : TC_Base
    {
        #region Constructors

        public TC_Create_Inheritance()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if a derived object is added well to one of its ancestors' set.";

            _testCaseDescriptor.Name = nameof(TC_Create_Inheritance);
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

                    if (teacher_actual.Age != _teacher.Age || teacher_actual.Name != _teacher.Name || teacher_actual.PlaceOfBirth != _teacher.PlaceOfBirth || teacher_actual.TeacherCode != _teacher.TeacherCode)
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
                _teacher.Age = 20;
                _teacher.Name = "Test Teacher 1";
                _teacher.PlaceOfBirth = "Test City 1";
                _teacher.TeacherCode = "Test Teacher Code 1";

                using (var universityContext = new UniversityContext())
                {
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

        private Teacher _teacher = new Teacher();

        #endregion
    }
}
