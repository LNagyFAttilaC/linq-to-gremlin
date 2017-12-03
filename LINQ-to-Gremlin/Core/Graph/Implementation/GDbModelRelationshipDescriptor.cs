using LINQtoGremlin.Core.Extensions.Internal;
using LINQtoGremlin.Core.Helper;
using System;

namespace LINQtoGremlin.Core.Graph
{
    public class GDbModelRelationshipDescriptor
        : IGDbModelRelationshipDescriptor
    {
        #region Constructors

        public GDbModelRelationshipDescriptor(
            IGDbModelDescriptor gDbModelDescriptor,
            string name,
            string property)
        {
            _gDbModelDescriptor = gDbModelDescriptor;
            _name = name;
            _property = property;

            if (!GDbModelDescriptor.TargetType
                .HasProperty(
                    property))
            {
                throw new ArgumentException(
                    "Type '" + GDbModelDescriptor.TargetType.Name + "' has no property with name '" + property + "'.",
                    nameof(property));
            }
        }

        #endregion

        #region Properties

        private GDbModelDescriptor GDbModelDescriptor
            => (GDbModelDescriptor)
                _gDbModelDescriptor;

        #endregion

        #region Methods

        public IGDbModelDescriptor ToMany(
            Type relatedType,
            string relatedProperty = null)
        {
            var typeOfProperty
                = GDbReflectionHelper
                    .GetTypeOfProperty(
                        GDbModelDescriptor.TargetType,
                        _property);

            if (typeOfProperty
                .IsCollectionType())
            {
                if (typeOfProperty
                    .GetCollectionType() == relatedType)
                {
                    if (relatedProperty != null)
                    {
                        if (relatedType
                            .HasProperty(
                                relatedProperty))
                        {
                            var typeOfRelatedProperty
                                = GDbReflectionHelper
                                    .GetTypeOfProperty(
                                        relatedType,
                                        relatedProperty);

                            if (typeOfProperty
                                .GetCollectionType() == GDbModelDescriptor.TargetType || !typeOfRelatedProperty
                                    .IsCollectionType())
                            {
                                if (typeOfRelatedProperty != GDbModelDescriptor.TargetType)
                                {
                                    if (typeOfRelatedProperty
                                        .GetCollectionType() != GDbModelDescriptor.TargetType)
                                    {
                                        throw new ArgumentException(
                                            "Incorrect related property: '" + relatedProperty + "' is not a '" + GDbModelDescriptor.TargetType.Name + "' type property.",
                                            nameof(relatedProperty));
                                    }
                                }
                            }
                            else
                            {
                                throw new ArgumentException(
                                    "Incorrect related property: '" + relatedProperty + "' is a collection. Many-to-many is not supported.",
                                    nameof(relatedProperty));
                            }
                        }
                        else
                        {
                            throw new ArgumentException(
                                "Incorrect related property: '" + relatedType.Name + "' has no property with name '" + relatedProperty + "'.",
                                nameof(relatedProperty));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(
                        "Incorrect related type: '" + relatedType.Name +"' for property '" + _property +"'.",
                        nameof(relatedType));
                }
            }
            else
            {
                throw new InvalidOperationException(
                    "Property '" + _property + "' is not a collection, so can not be used with 'ToMany()'.");
            }

            GDbModelDescriptor.AddModelDescriptorEntry(
                GDbModelDescriptorEntryTypes.EDGE,
                _name,
                _property,
                relatedProperty);

            return _gDbModelDescriptor;
        }

        public IGDbModelDescriptor ToOne(
            Type relatedType,
            string relatedProperty = null)
        {
            var typeOfProperty
                = GDbReflectionHelper
                    .GetTypeOfProperty(
                        GDbModelDescriptor.TargetType,
                        _property);

            if (!typeOfProperty
                .IsCollectionType())
            {
                    if (relatedProperty != null)
                    {
                        if (relatedType
                            .HasProperty(
                                relatedProperty))
                        {
                            var typeOfRelatedProperty
                                = GDbReflectionHelper
                                    .GetTypeOfProperty(
                                        relatedType,
                                        relatedProperty);

                            if (typeOfProperty == GDbModelDescriptor.TargetType || !typeOfRelatedProperty
                                .IsCollectionType())
                            {
                                if (typeOfRelatedProperty != GDbModelDescriptor.TargetType)
                                {
                                    throw new ArgumentException(
                                        "Incorrect related property: '" + relatedProperty + "' is not a '" + GDbModelDescriptor.TargetType.Name + "' type property.",
                                        nameof(relatedProperty));
                                }
                            }
                            else
                            {
                                throw new ArgumentException(
                                    "Incorrect related property: '" + relatedProperty + "' is a collection. Many-to-one should be defined on 'one' side with 'ToMany()'.",
                                    nameof(relatedProperty));
                            }
                        }
                        else
                        {
                            throw new ArgumentException(
                                "Incorrect related property: '" + relatedType.Name + "' has no property with name '" + relatedProperty + "'.",
                                nameof(relatedProperty));
                        }
                    }
            }
            else
            {
                throw new InvalidOperationException(
                    "Property '" + _property + "' is a collection, so can not be used with 'ToOne()'.");
            }

            GDbModelDescriptor.AddModelDescriptorEntry(
                GDbModelDescriptorEntryTypes.EDGE,
                _name,
                _property,
                relatedProperty);

            return _gDbModelDescriptor;
        }

        #endregion

        #region Fields

        private readonly IGDbModelDescriptor _gDbModelDescriptor;

        private readonly string _name;

        private readonly string _property;

        #endregion
    }
}
