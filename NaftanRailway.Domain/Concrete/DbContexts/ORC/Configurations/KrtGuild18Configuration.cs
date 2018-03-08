namespace NaftanRailway.Domain.Concrete.DbContexts.ORC.Configurations
{
    using System.Data.Entity.ModelConfiguration;
    using Entities;

    public class KrtGuild18Configuration : EntityTypeConfiguration<KrtGuild18>
    {
        public KrtGuild18Configuration()
        {
            this.ToTable("krt_Guild18");
            this.HasKey(t => t.Id);
        }
    }
}