using Railway.Core.Data.Interfaces.Factories;
using Railway.Domain.Concrete.DbContexts.ORC.Configurations;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.ORC
{
    public class OrcUnitOfWorkFactory : IUnitOfWorkFactory<IObdUnitOfWork>
    {
        private readonly string _connectionString;

        public OrcUnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IObdUnitOfWork Create()
        {
            var dbContext = new OrcDbContext(
                this._connectionString,
                builder =>
                {
                    builder.Configurations.Add(new KrtGuild18Configuration());
                });

            return new OrcUnitOfWork(dbContext);
        }
    }
}