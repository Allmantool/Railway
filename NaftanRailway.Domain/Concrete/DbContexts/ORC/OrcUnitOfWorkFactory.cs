namespace NaftanRailway.Domain.Concrete.DbContexts.ORC
{
    using Abstract;
    using Abstract.UnitOfWorks;
    using Configurations;

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