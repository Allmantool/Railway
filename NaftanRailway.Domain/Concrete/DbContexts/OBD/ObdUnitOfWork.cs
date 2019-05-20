namespace Railway.Domain.Concrete.DbContexts.OBD
{
    using System.Data.Entity;
    using Railway.Core.Data.EF;
    using Railway.Domain.Interfaces.UnitOfWorks;

    public class ObdUnitOfWork : UnitOfWork, IObdUnitOfWork
    {
        public ObdUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}