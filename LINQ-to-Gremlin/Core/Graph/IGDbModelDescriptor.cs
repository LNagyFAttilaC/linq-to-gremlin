using System;
using System.Collections.Generic;

namespace LINQtoGremlin.Core.Graph
{
    public interface IGDbModelDescriptor
    {
        IGDbModelDescriptor AddAncestor(
            Type type);

        IGDbModelRelationshipDescriptor AddEdge(
            string property);

        IGDbModelRelationshipDescriptor AddEdge(
            string name,
            string property);

        IGDbModelDescriptor AddProperty(
            string property);

        IGDbModelDescriptor AddProperty(
            string name,
            string property);

        IReadOnlyList<IGDbModelDescriptorEntry> GetModelDescriptorEntriesByType(
            GDbModelDescriptorEntryTypes gDbModelDescriptorEntryType);

        bool HasEdge(
            string edgeName);

        bool HasProperty(
            string propertyName);

        IGDbModelDescriptor SetLabel(
            string label);
    }
}
