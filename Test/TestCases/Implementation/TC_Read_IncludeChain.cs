using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Test.Context;
using Test.Model;
using Test.TestCases.Meta;

namespace Test.TestCases
{
    public class TC_Read_IncludeChain
        : TC_Base
    {
        #region Constructors

        public TC_Read_IncludeChain()
        {
            _testCaseDescriptor.Enabled = true;

            _testCaseDescriptor.Goal = "To check if include chains (Include + ThenInclude) works well.";

            _testCaseDescriptor.Name = nameof(TC_Read_IncludeChain);
        }

        #endregion

        #region Methods

        public override void Execute()
        {
            try
            {
                using (var universityContext = new UniversityContext())
                {
                    var universities
                        = universityContext.Universities
                            .Include(
                                u => u.Faculties)
                            .ThenInclude(
                                f => f.Departments)
                            .ToList();

                    if (universities.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Result set's cardinality is incorrect.\n" + universities.Count + " != 1");
                    }

                    var university_actual
                        = universities
                            .First();

                    if (university_actual.Faculties.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Included set's cardinality is incorrect.\n" + university_actual.Faculties.Count + " != 1");
                    }

                    var faculty_actual
                        = university_actual.Faculties
                            .First();

                    if (faculty_actual.Departments.Count != 1)
                    {
                        throw new TestCaseFailureException(
                            "Included set's cardinality is incorrect.\n" + faculty_actual.Departments.Count + " != 1");
                    }

                    var department_actual
                        = faculty_actual.Departments
                            .First();

                    if (university_actual.Id != _university.Id || faculty_actual.Id != _faculty.Id || department_actual.Id != _department.Id)
                    {
                        throw new TestCaseFailureException(
                            "Result entity's included entites are not perfect.");
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
                _university.FoundedIn = 2000;
                _university.Name = "Test University 1";
                _university.Faculties = new List<Faculty>();
                _university.Faculties
                    .Add(
                        _faculty);

                _faculty.FoundedIn = 2000;
                _faculty.Name = "Test Faculty 1";
                _faculty.Departments = new List<Department>();
                _faculty.Departments
                    .Add(
                        _department);
                _faculty.University = _university;

                _department.FoundedIn = 2000;
                _department.Name = "Test Department 1";
                _department.Faculty = _faculty;

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

        private University _university = new University();

        private Faculty _faculty = new Faculty();

        private Department _department = new Department();

        #endregion
    }
}
