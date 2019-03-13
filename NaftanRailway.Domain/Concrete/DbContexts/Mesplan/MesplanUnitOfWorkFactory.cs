using Railway.Domain.Concrete.DbContexts.Mesplan.Configurations;
using Railway.Domain.Interfaces;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.Mesplan
{
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