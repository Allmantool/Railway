using System.Data.Entity;
using Railway.Core.Data.EF;
using Railway.Domain.EF6;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.Mesplan
{
    public class MesplanUnitOfWork : UnitOfWork, IMesplanUnitOfWork
    {
        public MesplanUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}