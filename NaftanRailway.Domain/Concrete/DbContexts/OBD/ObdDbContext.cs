namespace NaftanRailway.Domain.Concrete.DbContexts.OBD
{
    using System;
    using System.Data.Entity;

    public class ObdDbContext : ConfigurableDbContext
    {
        public ObdDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<ObdDbContext>(null);
        }
    }
}