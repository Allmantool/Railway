namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan
{
    using System.Data.Entity;

    using Abstract.UnitOfWorks;

    public class MesplanUnitOfWork : UnitOfWork, IMesplanUnitOfWork
    {
        public MesplanUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}