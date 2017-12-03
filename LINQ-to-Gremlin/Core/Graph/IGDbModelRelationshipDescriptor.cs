using System;

namespace LINQtoGremlin.Core.Graph
{
    public interface IGDbModelRelationshipDescriptor
    {
        IGDbModelDescriptor ToMany(
            Type relatedType,
            string relatedProperty = null);

        IGDbModelDescriptor ToOne(
            Type relatedType,
            string relatedProperty = null);
    }
}
