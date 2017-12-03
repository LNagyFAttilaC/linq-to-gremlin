using LINQtoGremlin.Graph.GraphElements.Vertex;
using System;
using System.Collections.Generic;

namespace Test.Model
{
    public class Department
        : Vertex
    {
        #region Properties

        public Faculty Faculty
        {
            get;
            set;
        }

        public Guid FacultyId
        {
            get;
            set;
        }

        public int FoundedIn
        {
            get;
            set;
        }

        public Teacher Leader
        {
            get;
            set;
        }

        public Guid? LeaderId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IList<Subject> Subjects
        {
            get;
            set;
        }

        #endregion
    }
}
