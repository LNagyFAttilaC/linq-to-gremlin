using LINQtoGremlin.Graph.GraphElements.Vertex;
using System;
using System.Collections.Generic;

namespace Test.Model
{
    public class Faculty
        : Vertex
    {
        #region Properties

        public Teacher Dean
        {
            get;
            set;
        }

        public Guid? DeanId
        {
            get;
            set;
        }

        public IList<Department> Departments
        {
            get;
            set;
        }

        public int FoundedIn
        {
            get;
            set;
        }

        public string Name
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
