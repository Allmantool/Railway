namespace Railway.Domain.Concrete.DbContexts.Mesplan.Configurations
{
    using System.Data.Entity.ModelConfiguration;
    using Railway.Domain.Concrete.DbContexts.Mesplan.Entities;

    public class EtsngConfiguration : EntityTypeConfiguration<Etsng>
    {
        public EtsngConfiguration()
        {
            this.ToTable("etsng");
            this.HasKey(t => new { t.Etsng1, t.Name });
        }
    }
}