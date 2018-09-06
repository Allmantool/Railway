namespace NaftanRailway.Domain.Concrete.DbContexts.OBD
{
    using Abstract;
    using Abstract.UnitOfWorks;
    using Configurations;

    public class ObdUnitOfWorkFactory : IUnitOfWorkFactory<IObdUnitOfWork>
    {
        private readonly string _connectionString;

        public ObdUnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IObdUnitOfWork Create()
        {
            var dbContext = new ObdDbContext(
                this._connectionString,
                builder =>
                {
                    builder.Configurations.Add(new VPodhodConfiguration());
                });

            return new ObdUnitOfWork(dbContext);
        }
    }
}