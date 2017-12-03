using LINQtoGremlin.Core.Extensions;
using LINQtoGremlin.Core.Graph;
using LINQtoGremlin.Core.Storage.Internal;
using LINQtoGremlin.Graph.GraphElements.Vertex;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace LINQtoGremlin.Core.Infrastructure.Internal
{
    public class GDbContextOptionsExtension 
        : IDbContextOptionsExtension
        , IGDbContextOptionsExtension
    {
        #region Constructors

        public GDbContextOptionsExtension()
        {

        }

        public GDbContextOptionsExtension(
            GDbContextOptionsExtension other)
        {
            _gDbDatabaseCredentials = other._gDbDatabaseCredentials;
            _logFragment = other._logFragment;
            _modelDescriptors = other._modelDescriptors;
            _storeName = other._storeName;
        }

        #endregion

        #region Properties

        public virtual IGDbDatabaseCredentials DatabaseCredentials
            => _gDbDatabaseCredentials;

        public virtual string LogFragment
        {
            get
            {
                if (_logFragment == null)
                {
                    _logFragment 
                        = new StringBuilder()
                            .Append(
                                "StoreName = ")
                            .Append(
                                StoreName)
                            .Append(
                                " ")
                            .ToString();
                }

                return _logFragment;
            }
        }

        public virtual string StoreName
            => _storeName;

        #endregion

        #region Methods

        public virtual bool ApplyServices(
            IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddEntityFrameworkGDb();

            return true;
        }

        public virtual long GetServiceProviderHashCode()
            => 0L;

        public virtual void Validate(
            IDbContextOptions options)
        {
            
        }

        public virtual GDbContextOptionsExtension WithDatabaseCredentials(
            IGDbDatabaseCredentials gDbDatabaseCredentials)
        {
            var clone = Clone();

            clone._gDbDatabaseCredentials = gDbDatabaseCredentials;

            return clone;
        }

        public virtual GDbContextOptionsExtension WithStoreName(
            string storeName)
        {
            var clone = Clone();

            clone._storeName = storeName;

            return clone;
        }

        public virtual IGDbModelDescriptor GetModelDescriptor(
            Type type)
        {
            if (_modelDescriptors
                .ContainsKey(
                    type))
            {
                return _modelDescriptors[type];
            }
            else
            {
                return null;
            }
        }

        public virtual IGDbContextOptionsExtension Vertex(
            IGDbModelDescriptor modelDescriptor) 
        {
            if (modelDescriptor == null)
            {
                throw new ArgumentNullException(
                    nameof(modelDescriptor));
            }

            var targetType
                = ((GDbModelDescriptor)
                        modelDescriptor).TargetType;

            if (!typeof(Vertex)
                .IsAssignableFrom(
                    targetType))
            {
                throw new ArgumentException(
                    "Model descriptor's target type is not a child of Vertex.",
                    nameof(modelDescriptor));
            }

            _modelDescriptors
                .Add(
                    targetType, 
                    modelDescriptor
                        .AddAncestor(
                            typeof(Vertex)));

            return this;
        }

        protected virtual GDbContextOptionsExtension Clone()
            => new GDbContextOptionsExtension(
                this);

        #endregion

        #region Fields

        private IGDbDatabaseCredentials _gDbDatabaseCredentials;

        private string _logFragment;

        private readonly IDictionary<Type, IGDbModelDescriptor> _modelDescriptors = new Dictionary<Type, IGDbModelDescriptor>();

        private string _storeName;

        #endregion
    }
}
