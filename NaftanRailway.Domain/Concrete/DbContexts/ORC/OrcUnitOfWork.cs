using System.Data.Entity;
using Railway.Domain.EF6;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.ORC
{
    public class OrcUnitOfWork : EFUnitOfWork, IObdUnitOfWork
    {
        public OrcUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}