using System.Data.Entity;
using Railway.Domain.EF6;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.Mesplan
{
    public class MesplanUnitOfWork : EFUnitOfWork, IMesplanUnitOfWork
    {
        public MesplanUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}