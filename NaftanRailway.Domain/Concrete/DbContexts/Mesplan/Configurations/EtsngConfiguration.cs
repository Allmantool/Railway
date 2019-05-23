using System.Data.Entity.ModelConfiguration;
using Railway.Domain.Concrete.DbContexts.Mesplan.Entities;

namespace Railway.Domain.Concrete.DbContexts.Mesplan.Configurations
{
    public class EtsngConfiguration : EntityTypeConfiguration<Etsng>
    {
        public EtsngConfiguration()
        {
            this.ToTable("etsng");
            this.HasKey(t => new { t.Etsng1, t.Name });
        }
    }
}