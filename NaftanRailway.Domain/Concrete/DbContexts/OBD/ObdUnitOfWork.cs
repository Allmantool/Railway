using System.Data.Entity;
using Railway.Domain.EF6;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.OBD
{
    public class ObdUnitOfWork : EFUnitOfWork, IObdUnitOfWork
    {
        public ObdUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}