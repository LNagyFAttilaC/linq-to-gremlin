using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;

namespace LINQtoGremlin.Core.Graph.LanguageSpecific.GremlinQuery
{
    public static class GDbGremlinQueries
    {
        #region Methods

        public static GraphTraversal<Vertex, Edge> GetAddEdgeWithLabelFromVertexByIdToVertexByIdQuery(
            GraphTraversalSource g,
            object fromId,
            object toId,
            string label)
        {
            return g
                .V(
                    fromId)
                .As(
                    "from")
                .V(
                    toId)
                .AddE(
                    label)
                .From(
                    "from");
        }

        public static GraphTraversal<Vertex, Vertex> GetAddOrUpdatePropertyToOrOfVertexByIdQuery(
            GraphTraversalSource g,
            object id,
            string name,
            object value)
        {
            return g
                .V(
                    id)
                .Property(
                    name,
                    value);
        }

        public static GraphTraversal<Vertex, Vertex> GetAddVertexWithLabelQuery(
            GraphTraversalSource g,
            string label)
        {
            return g
                .AddV(
                    label);
        }

        public static GraphTraversal<Edge, Edge> GetDropEdgeByIdQuery(
            GraphTraversalSource g,
            object id)
        {
            return g
                .E(
                    id)
                .Drop();
        }

        public static GraphTraversal<Vertex, Vertex> GetDropVertexByIdQuery(
            GraphTraversalSource g,
            object id)
        {
            return g
                .V(
                    id)
                .Drop();
        }

        public static GraphTraversal<Vertex, Edge> GetGetOutEdgesByLabelOfVertexByIdQuery(
            GraphTraversalSource g,
            object id,
            string label)
        {
            return g
                .V(
                    id)
                .OutE(
                    label);
        }

        public static GraphTraversal<Vertex, Vertex> GetGetOutNeighboursByEdgeLabelOfVertexByIdQuery(
            GraphTraversalSource g,
            object id,
            string label)
        {
            return g
                .V(
                    id)
                .Out(
                    label);
        }

        public static GraphTraversal<Vertex, Vertex> GetGetOutNeighboursByVertexLabelOfVertexByIdQuery(
            GraphTraversalSource g,
            object id,
            string label)
        {
            return g
                .V(
                    id)
                .Out()
                .HasLabel(
                    label);
        }

        public static GraphTraversal<Vertex, Vertex> GetGetVerticesByLabelQuery(
            GraphTraversalSource g,
            string label)
        {
            return g
                .V()
                .HasLabel(
                    label);
        }

        #endregion
    }
}
