using LINQtoGremlin.Graph.GraphElements.Vertex;
using System;
using System.Collections.Generic;

namespace Test.Model
{
    public class University
        : Vertex
    {
        #region Properties

        public IList<Faculty> Faculties
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

        public Teacher Rector
        {
            get;
            set;
        }

        public Guid? RectorId
        {
            get;
            set;
        }

        #endregion
    }
}
