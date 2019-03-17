namespace Railway.Core.Data.EF.Builders
{
    using System.Data.Entity;
    using Railway.Core.Data.Interfaces.Builders;
    using Interfaces.Repositories;

    public class EFRepositoryBuilder<T, TDbContext> : IRepositoryBuilder<T, TDbContext>
        where T : class, new()
        where TDbContext : DbContext, new()
    {
        private IRepository<T> _repository;

        public IRepository<T> Build()
        {
            return _repository;
        }

        public IRepositoryBuilder<T, TDbContext> WithDbContext(TDbContext context)
        {
            _repository = new Repository<T>(context);

            return this;
        }
    }
}
