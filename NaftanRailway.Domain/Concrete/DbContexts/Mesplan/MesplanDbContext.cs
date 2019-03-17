namespace Railway.Domain.Concrete.DbContexts.Mesplan
{
    using System;
    using System.Data.Entity;
    using EF6;

    public class MesplanDbContext : ConfigurableDbContext
    {
        public MesplanDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<MesplanDbContext>(null);
        }
    }
}