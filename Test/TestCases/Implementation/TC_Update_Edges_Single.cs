using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Update_Edges_Single
        : TC_Base
    {
        #region Constructors

        public TC_Update_Edges_Single()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entity edges (relationships) are modified well. Using single relationship.";

            _testCaseDescriptor.Name = nameof(TC_Update_Edges_Single);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    // covering: modifying existing, included relationship

                    var universities1
                        = universityContext.Universities
                            .Include(
                                u => u.Rector)
                            .ToList();

                    if (universities1.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 1's cardinality is incorrect.\n" + universities1.Count + " != 1");
                    }

                    var university_actual1
                        = universities1
                            .First();

                    Teacher teacher = new Teacher();
                    teacher.Age = 30;
                    teacher.Name = "Test Teacher 1 2";
                    teacher.PlaceOfBirth = "Test City 1 2";
                    teacher.TeacherCode = "Test Teacher Code 1 2";

                    university_actual1.Rector = teacher;

                    universityContext
                        .Update(
                            university_actual1);

                    universityContext
                        .SaveChanges();

                    var universities2
                        = universityContext.Universities
                            .Include(
                                u => u.Rector)
                            .ToList();

                    if (universities2.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 2's cardinality is incorrect.\n" + universities2.Count + " != 1");
                    }

                    var university_actual2
                        = universities2
                            .First();

                    if (university_actual2.Rector.Id != teacher.Id)
                    {
                        throw new TestCaseFailureException(
                            "Modified result entity's edge is not perfect.");
                    }

                    // covering: deleting existing, included relationship

                    university_actual2.Rector = null;

                    universityContext
                        .Update(
                            university_actual2);

                    universityContext
                        .SaveChanges();

                    var universities3
                        = universityContext.Universities
                            .Include(
                                u => u.Rector)
                            .ToList();

                    if (universities3.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 3's cardinality is incorrect.\n" + universities3.Count + " != 1");
                    }

                    var university_actual3
                        = universities3
                            .First();

                    if (university_actual3.Rector != null)
                    {
                        throw new TestCaseFailureException(
                            "Modified result entity's edge is not perfect.");
                    }

                    // covering: adding to not existing, not included relationship

                    university_actual3.Rector = teacher;

                    universityContext
                        .Update(
                            university_actual3);

                    universityContext
                        .SaveChanges();

                    var universities4
                        = universityContext.Universities
                            .Include(
                                u => u.Rector)
                            .ToList();

                    if (universities4.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 4's cardinality is incorrect.\n" + universities4.Count + " != 1");
                    }

                    var university_actual4
                        = universities4
                            .First();

                    if (university_actual4.Rector.Id != teacher.Id)
                    {
                        throw new TestCaseFailureException(
                            "Modified result entity's edge is not perfect.");
                    }

                    // covering: do nothing with existing, not included relationship

                    var universities5
                        = universityContext.Universities
                            .ToList();

                    if (universities5.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 5's cardinality is incorrect.\n" + universities5.Count + " != 1");
                    }

                    var university_actual5
                        = universities5
                            .First();

                    universityContext
                        .Update(
                            university_actual5);

                    universityContext
                        .SaveChanges();

                    var universities6
                        = universityContext.Universities
                            .Include(
                                u => u.Rector)
                            .ToList();

                    if (universities6.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 6's cardinality is incorrect.\n" + universities6.Count + " != 1");
                    }

                    var university_actual6
                        = universities6
                            .First();

                    if (university_actual6.Rector.Id != teacher.Id)
                    {
                        throw new TestCaseFailureException(
                            "Modified result entity's edge is not perfect.");
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
                Teacher teacher = new Teacher();

                University university = new University();

                teacher.Age = 20;
                teacher.Name = "Test Teacher 1";
                teacher.PlaceOfBirth = "Test City 1";
                teacher.TeacherCode = "Test Teacher Code 1";

                university.FoundedIn = 2000;
                university.Name = "Test University 1";
                university.Rector = teacher;

                using (var universityContext = new UniversityContext())
                {
                    universityContext.Universities
                        .Add(
                            university);

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
