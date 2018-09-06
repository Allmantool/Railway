namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan
{
    using Abstract;
    using Abstract.UnitOfWorks;
    using Configurations;

    public class MesplanUnitOfWorkFactory : IUnitOfWorkFactory<IMesplanUnitOfWork>
    {
        private readonly string _connectionString;

        public MesplanUnitOfWorkFactory(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public IMesplanUnitOfWork Create()
        {
            var databaseContext = new MesplanDbContext(
                this._connectionString,
                builder =>
                {
                    builder.Configurations.Add(new EtsngConfiguration());
                });

            return new MesplanUnitOfWork(databaseContext);
        }
    }
}