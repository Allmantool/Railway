using System;
using System.Data.Entity;
using Railway.Domain.EF6;

namespace Railway.Domain.Concrete.DbContexts.OBD
{
    public class ObdDbContext : ConfigurableDbContext
    {
        public ObdDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString, configurationApplicator)
        {
            Database.SetInitializer<ObdDbContext>(null);
        }
    }
}