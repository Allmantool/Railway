namespace NaftanRailway.Domain.Concrete.DbContexts.ORC
{
    using System;
    using System.Data.Entity;

    public class OrcDbContext : ConfigurableDbContext
    {
        public OrcDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<OrcDbContext>(null);
        }
    }
}