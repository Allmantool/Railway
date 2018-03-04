namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan
{
    using NaftanRailway.Domain.Abstract;
    using NaftanRailway.Domain.Abstract.UnitOfWorks;
    using NaftanRailway.Domain.Concrete.DbContexts.Mesplan.Configuration;

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
                _connectionString,
                builder =>
                {
                    builder.Configurations.Add(new EtsngConfiguration());
                });

            return new MesplanUnitOfWork(databaseContext);
        }
    }
}