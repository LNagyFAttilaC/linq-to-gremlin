using LINQtoGremlin.Core.Graph;
using System;

namespace LINQtoGremlin.Core.Infrastructure.Internal
{
    public interface IGDbContextOptionsExtension
    {
        IGDbModelDescriptor GetModelDescriptor(
            Type type);

        IGDbContextOptionsExtension Vertex(
            IGDbModelDescriptor modelDescriptor);
    }
}
