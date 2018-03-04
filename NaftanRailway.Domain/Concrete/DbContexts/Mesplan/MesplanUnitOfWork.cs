namespace NaftanRailway.Domain.Concrete.DbContexts.Mesplan
{
    using System.Data.Entity;

    using NaftanRailway.Domain.Abstract.UnitOfWorks;

    public class MesplanUnitOfWork : UnitOfWork, IMesplanUnitOfWork
    {
        public MesplanUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}