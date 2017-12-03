using Microsoft.EntityFrameworkCore;

namespace LINQtoGremlin.Core.Infrastructure
{
    public class GDbContextOptionsBuilder 
        : IGDbContextOptionsBuilder
    {
        #region Constructors

        public GDbContextOptionsBuilder(
            DbContextOptionsBuilder dbContextOptionsBuilder)
            => DbContextOptionsBuilder = dbContextOptionsBuilder;

        #endregion

        #region Properties

        protected virtual DbContextOptionsBuilder DbContextOptionsBuilder
        {
            get;
        }

        #endregion
    }
}
