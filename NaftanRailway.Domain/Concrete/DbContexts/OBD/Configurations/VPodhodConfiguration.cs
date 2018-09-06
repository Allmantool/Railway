namespace NaftanRailway.Domain.Concrete.DbContexts.OBD.Configurations
{
    using System.Data.Entity.ModelConfiguration;
    using Entities;

    public class VPodhodConfiguration : EntityTypeConfiguration<VPodhod>
    {
        public VPodhodConfiguration()
        {
            this.ToTable("v_02_podhod");
            this.HasKey(t => t.n_vag);
        }
    }
}