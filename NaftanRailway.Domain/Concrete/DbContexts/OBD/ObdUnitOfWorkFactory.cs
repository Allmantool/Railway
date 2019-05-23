using Railway.Core.Data.Interfaces.Factories;

namespace Railway.Domain.Concrete.DbContexts.OBD
{
    using Interfaces;
    using Interfaces.UnitOfWorks;

    public class ObdUnitOfWorkFactory : IUnitOfWorkFactory<IObdUnitOfWork>
    {
        private readonly string connectionString;

        public ObdUnitOfWorkFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IObdUnitOfWork Create()
        {
            var dbContext = new ObdDbContext(
                this.connectionString,
                builder =>
                {
                    // builder.Configurations.Add();
                });

            return new ObdUnitOfWork(dbContext);
        }
    }
}