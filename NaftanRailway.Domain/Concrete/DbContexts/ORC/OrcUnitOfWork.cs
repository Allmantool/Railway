namespace NaftanRailway.Domain.Concrete.DbContexts.ORC
{
    using System.Data.Entity;
    using Abstract.UnitOfWorks;

    public class OrcUnitOfWork : UnitOfWork, IObdUnitOfWork
    {
        public OrcUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}