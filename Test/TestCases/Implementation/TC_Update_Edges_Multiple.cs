using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Update_Edges_Multiple
        : TC_Base
    {
        #region Constructors

        public TC_Update_Edges_Multiple()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if entity edges (relationships) are modified well. Using multiple relationship.";

            _testCaseDescriptor.Name = nameof(TC_Update_Edges_Multiple);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    // covering: modifying (deleting and adding to) existing, included relationship

                    var departments1
                        = universityContext.Departments
                            .Include(
                                u => u.Subjects)
                            .ToList();

                    if (departments1.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 1's cardinality is incorrect.\n" + departments1.Count + " != 1");
                    }

                    var department_actual1
                        = departments1
                            .First();

                    department_actual1.Subjects
                        = department_actual1.Subjects
                            .OrderBy(
                                s => s.Name)
                            .ToList();

                    var subject2_actual1 = department_actual1.Subjects[1];

                    Subject subject = new Subject();
                    subject.Name = "Test Subject 1 2";

                    department_actual1.Subjects
                        .Remove(
                            subject2_actual1);

                    department_actual1.Subjects
                        .Add(
                            subject);

                    universityContext
                        .Update(
                            department_actual1);

                    universityContext
                        .SaveChanges();

                    universityContext
                        .Entry(
                            subject).State = EntityState.Detached;

                    var departments2
                        = universityContext.Departments
                            .Include(
                                u => u.Subjects)
                            .ToList();

                    if (departments2.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 2's cardinality is incorrect.\n" + departments2.Count + " != 1");
                    }

                    var department_actual2
                        = departments2
                            .First();

                    department_actual2.Subjects
                        = department_actual2.Subjects
                            .OrderBy(
                                s => s.Name)
                            .ToList();

                    if (department_actual2.Subjects.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set 2's edges' cardinality is incorrect.\n" + department_actual2.Subjects.Count + " != 2");
                    }

                    if (department_actual2.Subjects[0].Id != department_actual1.Subjects[0].Id || department_actual2.Subjects[1].Id != subject.Id)
                    {
                        throw new TestCaseFailureException(
                            "Modified result entity's edge is not perfect.");
                    }

                    // covering: do nothing with existing, not included relationship

                    var departments3
                        = universityContext.Departments
                            .ToList();

                    if (departments3.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 3's cardinality is incorrect.\n" + departments3.Count + " != 1");
                    }

                    var department_actual3
                        = departments3
                            .First();

                    universityContext
                        .Update(
                            department_actual3);

                    universityContext
                        .SaveChanges();

                    var departments4
                        = universityContext.Departments
                            .Include(
                                u => u.Subjects)
                            .ToList();

                    if (departments4.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set 4's cardinality is incorrect.\n" + departments4.Count + " != 1");
                    }

                    var department_actual4
                        = departments4
                            .First();

                    if (department_actual4.Subjects.Count != 2)
                    {
                        throw new TestCaseFailureException(
                            "Result set 4's edges' cardinality is incorrect.\n" + department_actual4.Subjects.Count + " != 2");
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
                Department department = new Department();

                Subject subject1 = new Subject();
                Subject subject2 = new Subject();

                department.FoundedIn = 2000;
                department.Name = "Test Department 1";
                department.Subjects = new List<Subject>();
                department.Subjects
                    .Add(
                        subject1);
                department.Subjects
                    .Add(
                        subject2);

                subject1.Name = "Test Subject 1";

                subject2.Name = "Test Subject 2";

                using (var universityContext = new UniversityContext())
                {
                    universityContext.Departments
                        .Add(
                            department);

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
