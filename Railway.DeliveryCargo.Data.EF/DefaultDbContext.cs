namespace Railway.DeliveryCargo.Data.EF
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public class DefaultDbContext : ConfigurableDbContext
    {
        public DefaultDbContext(DbContextOptions dbContextOptions, Action<ModelBuilder> configurationApplicator)
            : base(dbContextOptions, configurationApplicator)
        {
        }
    }
}
