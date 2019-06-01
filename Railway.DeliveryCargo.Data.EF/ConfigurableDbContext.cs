namespace Railway.DeliveryCargo.Data.EF
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Railway.DeliveryCargo.Data.EF.Domain;
    using Railway.DeliveryCargo.Data.EF.Factories;

    public abstract class ConfigurableDbContext : DbContext
    {
        protected ConfigurableDbContext(DbContextOptions dbContextOptions, Action<ModelBuilder> configurationApplicator)
            : base(dbContextOptions)
        {
            ConfigurationApplicator = configurationApplicator;
        }

        private Action<ModelBuilder> ConfigurationApplicator { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SqlDatabaseSchemas.Dbo);
            ConfigurationApplicator(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        #if DEBUG
            optionsBuilder.UseLoggerFactory(LoggerFactorySingleton.Instance)
                .EnableSensitiveDataLogging();
        #endif
        }
    }
}
