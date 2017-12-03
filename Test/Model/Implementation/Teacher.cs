using System;
using System.Collections.Generic;

namespace Test.Model
{
    public class Teacher
        : Person
    {
        #region Properties

        public Department LeaderAtDepartment
        {
            get;
            set;
        }

        public Guid LeaderAtDepartmentId
        {
            get;
            set;
        }

        public Faculty DeanAtFaculty
        {
            get;
            set;
        }

        public Guid DeanAtFacultyId
        {
            get;
            set;
        }

        public University RectorAtUniversity
        {
            get;
            set;
        }

        public Guid RectorAtUniversityId
        {
            get;
            set;
        }

        public IList<Subject_Teacher> Subjects
        {
            get;
            set;
        }

        public string TeacherCode
        {
            get;
            set;
        }

        public University University
        {
            get;
            set;
        }

        public Guid UniversityId
        {
            get;
            set;
        }

        #endregion
    }
}
