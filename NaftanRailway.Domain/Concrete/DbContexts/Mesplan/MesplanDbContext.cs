using System;
using System.Data.Entity;
using Railway.Domain.EF6;

namespace Railway.Domain.Concrete.DbContexts.Mesplan
{
    public class MesplanDbContext : ConfigurableDbContext
    {
        public MesplanDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<MesplanDbContext>(null);
        }
    }
}