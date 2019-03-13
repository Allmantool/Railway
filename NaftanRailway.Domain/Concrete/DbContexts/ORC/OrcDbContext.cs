using System;
using System.Data.Entity;
using Railway.Domain.EF6;

namespace Railway.Domain.Concrete.DbContexts.ORC
{
    public class OrcDbContext : ConfigurableDbContext
    {
        public OrcDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<OrcDbContext>(null);
        }
    }
}