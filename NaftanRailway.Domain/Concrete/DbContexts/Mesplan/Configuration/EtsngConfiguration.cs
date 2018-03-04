namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan.Configuration
{
    using System.Data.Entity.ModelConfiguration;

    using NaftanRailway.Domain.Concrete.DbContexts.Mesplan.Entities;

    public class EtsngConfiguration : EntityTypeConfiguration<Etsng>
    {
        public EtsngConfiguration()
        {
            this.ToTable("etsng");
            this.HasKey(t => new { t.Etsng1, t.Name });
        }
    }
}