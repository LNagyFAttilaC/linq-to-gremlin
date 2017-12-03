using LINQtoGremlin.Graph.GraphElements.Vertex;
using Microsoft.EntityFrameworkCore;

namespace LINQtoGremlin.Core.Graph
{
    public interface IGDbContext
    {
        DbSet<Vertex> Vertices
        {
            get;
            set;
        }
    }
}
