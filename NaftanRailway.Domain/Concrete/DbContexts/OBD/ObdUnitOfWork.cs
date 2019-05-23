namespace Railway.Domain.Concrete.DbContexts.OBD
{
    using System.Data.Entity;
    using Core.Data.EF;
    using Interfaces.UnitOfWorks;

    public class ObdUnitOfWork : UnitOfWork, IObdUnitOfWork
    {
        public ObdUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}