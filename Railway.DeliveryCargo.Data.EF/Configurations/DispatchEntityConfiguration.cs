namespace Railway.DeliveryCargo.Data.EF.Configurations
{
    using Dto;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DispatchEntityConfiguration : IEntityTypeConfiguration<DispatchEntity>
    {
        public void Configure(EntityTypeBuilder<DispatchEntity> builder)
        {
            builder.ToTable("DISPATCHES");
            builder.HasKey(e => e.Id);
        }
    }
}
