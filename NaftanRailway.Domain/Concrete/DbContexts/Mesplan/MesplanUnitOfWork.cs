namespace Railway.Domain.Concrete.DbContexts.Mesplan
{
    using System.Data.Entity;
    using Railway.Core.Data.EF;
    using Railway.Domain.Interfaces.UnitOfWorks;

    public class MesplanUnitOfWork : UnitOfWork, IMesplanUnitOfWork
    {
        public MesplanUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}