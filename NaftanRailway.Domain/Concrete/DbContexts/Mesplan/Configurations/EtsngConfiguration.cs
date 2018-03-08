using System.Data.Entity.ModelConfiguration;
using NaftanRailway.Domain.Concrete.DbContexts.Mesplan.Entities;

namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan.Configurations
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