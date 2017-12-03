using LINQtoGremlin.Core.Properties;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Update;
using System.Collections.Generic;

namespace LINQtoGremlin.Core.Extensions.Internal
{
    public static class GDbExtensionsOfIDiagnosticsLogger
    {
        #region Methods

        public static void ChangesSaved(
            this IDiagnosticsLogger<DbLoggerCategory.Update> diagnosticsLogger,
            IEnumerable<IUpdateEntry> entries,
            int entriesAffected)
        {
            var definition = GdbStrings.LogChangesSaved;

            definition
                .Log(
                    diagnosticsLogger, 
                    entriesAffected);

            if (diagnosticsLogger.DiagnosticSource
                .IsEnabled(
                    definition.EventId.Name))
            {
                diagnosticsLogger.DiagnosticSource
                    .Write(
                        definition.EventId.Name,
                        new SaveChangesEventData(
                            definition,
                            ChangesSaved,
                            entries,
                            entriesAffected));
            }
        }

        public static void DatabaseTransactionIgnoredWarning(
            this IDiagnosticsLogger<DbLoggerCategory.Database.Transaction> diagnosticsLogger)
        {
            var definition = GdbStrings.LogDatabaseTransactionIgnoredWarning;

            definition
                .Log(
                    diagnosticsLogger);

            if (diagnosticsLogger.DiagnosticSource
                .IsEnabled(
                    definition.EventId.Name))
            {
                diagnosticsLogger.DiagnosticSource
                    .Write(
                        definition.EventId.Name,
                        new EventData(
                            definition,
                            (d, p) => ((EventDefinition)
                                d)
                                .GenerateMessage()));
            }
        }

        private static string ChangesSaved(
            EventDefinitionBase eventDefinitionBase, 
            EventData eventData)
        {
            return ((EventDefinition<int>)
                eventDefinitionBase)
                .GenerateMessage(
                    ((SaveChangesEventData)
                        eventData)
                        .RowsAffected);
        }

        #endregion
    }
}
