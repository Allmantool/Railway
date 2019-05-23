using System.Data.Entity;
using Railway.Core.Data.EF;
using Railway.Domain.EF6;
using Railway.Domain.Interfaces.UnitOfWorks;

namespace Railway.Domain.Concrete.DbContexts.ORC
{
    public class OrcUnitOfWork : UnitOfWork, IObdUnitOfWork
    {
        public OrcUnitOfWork(DbContext context)
            : base(context)
        {
        }
    }
}