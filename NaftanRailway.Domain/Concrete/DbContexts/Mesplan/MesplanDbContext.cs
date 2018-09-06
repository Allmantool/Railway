namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan
{
    using System;
    using System.Data.Entity;

    public class MesplanDbContext : ConfigurableDbContext
    {
        public MesplanDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<MesplanDbContext>(null);
        }
    }
}