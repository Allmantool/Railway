using System.Data.Entity.Infrastructure.Interception;

namespace NaftanRailway.Domain.DbConfiguration {
    /// <summary>
    /// More flexible loggging for EF6 then Database.Log
    /// http://www.mortenanderson.net/logging-sql-statements-in-entity-framework-with-interception
    /// https://msdn.microsoft.com/en-us/data/jj680699.aspx
    /// </summary>
    public class Configuration : System.Data.Entity.DbConfiguration {
        public Configuration() {
            DbInterception.Add(new EFInterceptor());
        }
    }
}