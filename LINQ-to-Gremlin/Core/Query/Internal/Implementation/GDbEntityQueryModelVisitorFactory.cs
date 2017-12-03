using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;

namespace LINQtoGremlin.Core.Query.Internal
{
    public class GDbEntityQueryModelVisitorFactory 
        : EntityQueryModelVisitorFactory
        , IGDbEntityQueryModelVisitorFactory
    {
        #region Constructors

        public GDbEntityQueryModelVisitorFactory(
            EntityQueryModelVisitorDependencies entityQueryModelVisitorDependencies,
            IDiagnosticsLogger<DbLoggerCategory.Query> diagnosticsLogger)
            : base(
                  entityQueryModelVisitorDependencies)
            => _diagnosticsLogger = diagnosticsLogger;

        #endregion

        #region Methods

        public override EntityQueryModelVisitor Create(
            QueryCompilationContext queryCompilationContext,
            EntityQueryModelVisitor entityQueryModelVisitor)
            => new GDbEntityQueryModelVisitor(
                Dependencies, 
                queryCompilationContext, 
                _diagnosticsLogger);

        #endregion

        #region Fields

        private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _diagnosticsLogger;

        #endregion
    }
}
