using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LINQtoGremlin.Core.Graph
{
    public class GDbModelDescriptor 
        : IGDbModelDescriptor
    {
        #region Constructors

        public GDbModelDescriptor(
            Type targetType)
            => _targetType = targetType;

        #endregion

        #region Properties

        public Type TargetType
            => _targetType;

        #endregion

        #region Methods

        public IGDbModelDescriptor AddAncestor(
            Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(
                    nameof(type));
            }

            AddModelDescriptorEntry(
                GDbModelDescriptorEntryTypes.ANCESTOR,
                null,
                type);

            return this;
        }

        public IGDbModelRelationshipDescriptor AddEdge(
            string property)
            => AddEdge(
                property,
                property);

        public IGDbModelRelationshipDescriptor AddEdge(
            string name, 
            string property)
        {
            if (name == null)
            {
                throw new ArgumentNullException(
                    nameof(name));
            }

            if (property == null)
            {
                throw new ArgumentNullException(
                    nameof(property));
            }

            if (HasEdge(
                name) || HasProperty(
                    name))
            {
                throw new ArgumentException(
                    "Edge with name '" + name + "' can not be added, because that name is already in use.",
                    nameof(name));
            }

            return new GDbModelRelationshipDescriptor(
                this,
                name,
                property);
        }

        public IGDbModelDescriptor AddProperty(
            string property)
            => AddProperty(
                property,
                property);

        public IGDbModelDescriptor AddProperty(
            string name, 
            string property)
        {
            if (name == null)
            {
                throw new ArgumentNullException(
                    nameof(name));
            }

            if (property == null)
            {
                throw new ArgumentNullException(
                    nameof(property));
            }

            if (HasProperty(
                name) || HasEdge(
                    name))
            {
                throw new ArgumentException(
                    "Property with name '" + name + "' can not be added, because that name is already in use.",
                    nameof(name));
            }

            AddModelDescriptorEntry(
                GDbModelDescriptorEntryTypes.PROPERTY,
                name,
                property);

            return this;
        }

        public IReadOnlyList<IGDbModelDescriptorEntry> GetModelDescriptorEntriesByType(
            GDbModelDescriptorEntryTypes gDbModelDescriptorEntryType)
        {
            if (_gDbModelDescriptorEntries
                .ContainsKey(
                    gDbModelDescriptorEntryType))
            {
                return new ReadOnlyCollection<IGDbModelDescriptorEntry>(
                    _gDbModelDescriptorEntries[gDbModelDescriptorEntryType]);
            }
            else
            {
                return new ReadOnlyCollection<IGDbModelDescriptorEntry>(
                    new List<IGDbModelDescriptorEntry>());
            }
        }

        public bool HasEdge(
            string edgeName)
        {
            if (_gDbModelDescriptorEntries
                .ContainsKey(
                    GDbModelDescriptorEntryTypes.EDGE))
            {
                foreach (var gDbModelDescriptorEntry in _gDbModelDescriptorEntries[GDbModelDescriptorEntryTypes.EDGE])
                {
                    if (gDbModelDescriptorEntry.EntryValue
                            .ToString() == edgeName)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        public bool HasProperty(
            string propertyName)
        {
            if (_gDbModelDescriptorEntries
                .ContainsKey(
                    GDbModelDescriptorEntryTypes.PROPERTY))
            {
                foreach (var gDbModelDescriptorEntry in _gDbModelDescriptorEntries[GDbModelDescriptorEntryTypes.PROPERTY])
                {
                    if (gDbModelDescriptorEntry.EntryValue
                            .ToString() == propertyName)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        public IGDbModelDescriptor SetLabel(
            string label)
            => AddLabel(
                label,
                true);

        internal IGDbModelDescriptor AddLabel(
            string label,
            bool checkIfReserved)
        {
            if (label == null)
            {
                throw new ArgumentNullException(
                    nameof(label));
            }

            if (checkIfReserved && (label == GDbModelConstants._INNER_PROPERTY_VERTEX_NAME || label == GDbModelConstants._INNER_VERTEX_NAME))
            {
                throw new ArgumentException(
                    "Label '" + label + "' is reserved for inner use.",
                    nameof(label));
            }

            if (_gDbModelDescriptorEntries
                .ContainsKey(
                    GDbModelDescriptorEntryTypes.LABEL))
            {
                _gDbModelDescriptorEntries[GDbModelDescriptorEntryTypes.LABEL]
                    .Clear();
            }

            AddModelDescriptorEntry(
                GDbModelDescriptorEntryTypes.LABEL,
                null,
                label);

            return this;
        }

        internal IGDbModelDescriptor AddModelDescriptorEntry(
            GDbModelDescriptorEntryTypes gDbModelDescriptorEntryType,
            string modelDescriptorEntryName,
            object modelDescriptorEntryValue,
            object modelDescriptorExtra = null)
        {
            GDbModelDescriptorEntry gDbModelDescriptorEntry
                = new GDbModelDescriptorEntry()
                {
                    EntryName = modelDescriptorEntryName,
                    EntryValue = modelDescriptorEntryValue,
                    Extra = modelDescriptorExtra
                };

            if (_gDbModelDescriptorEntries
                .ContainsKey(
                    gDbModelDescriptorEntryType))
            {
                if (!_gDbModelDescriptorEntries[gDbModelDescriptorEntryType]
                    .Contains(
                        gDbModelDescriptorEntry))
                {
                    _gDbModelDescriptorEntries[gDbModelDescriptorEntryType]
                        .Add(
                            gDbModelDescriptorEntry);
                }
            }
            else
            {
                _gDbModelDescriptorEntries.Add(
                    gDbModelDescriptorEntryType,
                    new List<IGDbModelDescriptorEntry>() {
                        gDbModelDescriptorEntry
                    });
            }

            return this;
        }

        #endregion

        #region Fields

        private readonly IDictionary<GDbModelDescriptorEntryTypes, IList<IGDbModelDescriptorEntry>> _gDbModelDescriptorEntries = new Dictionary<GDbModelDescriptorEntryTypes, IList<IGDbModelDescriptorEntry>>();

        private readonly Type _targetType;

        #endregion
    }
}
