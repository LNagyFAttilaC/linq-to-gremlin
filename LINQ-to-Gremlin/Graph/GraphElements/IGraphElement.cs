using System.Collections.Generic;

namespace LINQtoGremlin.Graph.GraphElements
{
    public interface IGraphElement
    {
        IDictionary<string, object> GetExtraProperties();

        bool HasProperty(
            string propertyName);

        object this[
            string propertyName]
        {
            get;
            set;
        }
    }
}
