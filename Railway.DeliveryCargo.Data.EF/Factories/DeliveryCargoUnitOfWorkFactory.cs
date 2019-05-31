namespace Railway.DeliveryCargo.Data.EF.Factories
{
    using Microsoft.EntityFrameworkCore;
    using Railway.DeliveryCargo.Data.EF.Configurations;
    using Railway.DeliveryCargo.Data.EF.Interfaces;
    using Railway.DeliveryCargo.Data.EF.Interfaces.Factories;

    public class DeliveryCargoUnitOfWorkFactory : IUnitOfWorkFactory<IUnitOfWork>
    {
        private readonly string _connectionString;

        public DeliveryCargoUnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IUnitOfWork Create()
        {
            var optionBuilder = new DbContextOptionsBuilder()
                .UseSqlServer(_connectionString);

            var databaseContext = new DefaultDbContext(
                optionBuilder.Options,
                builder =>
                {
                    builder.ApplyConfiguration(new DispatchEntityConfiguration());
                });

            return new UnitOfWork(databaseContext, _connectionString);
        }
    }
}
