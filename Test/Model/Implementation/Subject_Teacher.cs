using LINQtoGremlin.Graph.GraphElements.Vertex;
using System;

namespace Test.Model
{
    public class Subject_Teacher
        : Vertex
    {
        #region Properties

        public Subject Subject
        {
            get;
            set;
        }

        public Guid SubjectId
        {
            get;
            set;
        }

        public Teacher Teacher
        {
            get;
            set;
        }

        public Guid TeacherId
        {
            get;
            set;
        }

        #endregion
    }
}
