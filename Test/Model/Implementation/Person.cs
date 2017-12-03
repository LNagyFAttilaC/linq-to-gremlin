using LINQtoGremlin.Graph.GraphElements.Vertex;

namespace Test.Model
{
    public class Person
        : Vertex
    {
        #region Properties

        public int Age
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string PlaceOfBirth
        {
            get;
            set;
        }

        #endregion
    }
}
