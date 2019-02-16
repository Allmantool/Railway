namespace NaftanRailway.Domain.Concrete.DbContexts.OBD
{
    using System.Data.Entity;
    using Abstract.UnitOfWorks;

    public class ObdUnitOfWork : UnitOfWork, IObdUnitOfWork
    {
        public ObdUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}