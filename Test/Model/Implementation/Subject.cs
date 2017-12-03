using LINQtoGremlin.Graph.GraphElements.Vertex;
using System;
using System.Collections.Generic;

namespace Test.Model
{
    public class Subject
        : Vertex
    {
        #region Properties

        public string Name
        {
            get;
            set;
        }

        public Department Department
        {
            get;
            set;
        }

        public Guid DepartmentId
        {
            get;
            set;
        }


        public IList<Subject_Teacher> Teachers
        {
            get;
            set;
        }

        #endregion
    }
}
