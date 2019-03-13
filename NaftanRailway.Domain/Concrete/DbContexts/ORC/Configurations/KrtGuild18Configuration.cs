using System.Data.Entity.ModelConfiguration;
using Railway.Domain.Concrete.DbContexts.ORC.Entities;

namespace Railway.Domain.Concrete.DbContexts.ORC.Configurations
{
    public class KrtGuild18Configuration : EntityTypeConfiguration<KrtGuild18>
    {
        public KrtGuild18Configuration()
        {
            this.ToTable("krt_Guild18");
            this.HasKey(t => t.Id);
        }
    }
}