namespace NaftanRailway.Domain.Concrete
{
    using System;
    using System.Data.Entity;

    public abstract class ConfigurableDbContext : DbContext
    {
        protected ConfigurableDbContext(string connectionString, Action<DbModelBuilder> configurationApplicator)
            : base(connectionString)
        {
            this.ConfigurationApplicator = configurationApplicator;
#if DEBUG
            this.Database.Log = (sql) => Debug.WriteLine(sql);
#endif
        }

        private Action<DbModelBuilder> ConfigurationApplicator { get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            this.ConfigurationApplicator(modelBuilder);
        }
    }
}