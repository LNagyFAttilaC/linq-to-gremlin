using LINQtoGremlin.Core.Diagnostics;
using LINQtoGremlin.Core.Infrastructure;
using LINQtoGremlin.Core.Infrastructure.Internal;
using LINQtoGremlin.Core.Storage.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace LINQtoGremlin.Core.Extensions
{
    public static class GDbExtensionsOfDbContextOptionsBuilder
    {
        #region Methods

        public static DbContextOptionsBuilder<TContext> UseGDatabase<TContext>(
            this DbContextOptionsBuilder<TContext> dbContextOptionsBuilder,
            string storeName,
            IGDbDatabaseCredentials gDbDatabaseCredentials,
            Action<GDbContextOptionsBuilder> gDbContextOptionsAction = null)
            where TContext 
                : DbContext
            => (DbContextOptionsBuilder<TContext>)
                    UseGDatabase(
                        (DbContextOptionsBuilder)
                            dbContextOptionsBuilder, 
                        storeName,
                        gDbDatabaseCredentials);

        public static DbContextOptionsBuilder UseGDatabase(
            this DbContextOptionsBuilder dbContextOptionsBuilder,
            string storeName,
            IGDbDatabaseCredentials gDbDatabaseCredentials,
            Action<GDbContextOptionsBuilder> gDbContextOptionsAction = null)
        {
            var extension 
                = dbContextOptionsBuilder.Options
                    .FindExtension<GDbContextOptionsExtension>() 
                        ?? new GDbContextOptionsExtension();

            extension = extension
                .WithStoreName(
                    storeName)
                .WithDatabaseCredentials(
                    gDbDatabaseCredentials);

            ConfigureWarnings(
                dbContextOptionsBuilder);

            ((IDbContextOptionsBuilderInfrastructure)
                dbContextOptionsBuilder)
                .AddOrUpdateExtension(
                    extension);

            return dbContextOptionsBuilder;
        }

        public static GDbContextOptionsExtension GetGDbContextOptionsExtension(
            this DbContextOptionsBuilder dbContextOptionsBuilder)
            => dbContextOptionsBuilder.Options
                .GetExtension<GDbContextOptionsExtension>();

        private static void ConfigureWarnings(
            DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            var extension
                = dbContextOptionsBuilder.Options
                    .FindExtension<CoreOptionsExtension>()
                        ?? new CoreOptionsExtension();

            extension = extension
                .WithWarningsConfiguration(
                    extension.WarningsConfiguration
                        .TryWithExplicit(
                            GDbEventId._databaseTransactionIgnoredWarning, 
                            WarningBehavior.Throw));

            ((IDbContextOptionsBuilderInfrastructure)
                dbContextOptionsBuilder)
                .AddOrUpdateExtension(
                    extension);
        }

        #endregion
    }
}
