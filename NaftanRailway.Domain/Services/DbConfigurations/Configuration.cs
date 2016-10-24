using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity;

namespace NaftanRailway.Domain.DbConfigurations {
    /// <summary>
    /// More flexible loggging for EF6 then Database.Log
    /// !!!Warning!!!! It's possible Conflict with registration in web.config or body code (double log)
    /// http://www.mortenanderson.net/logging-sql-statements-in-entity-framework-with-interception
    /// https://msdn.microsoft.com/en-us/data/jj680699.aspx
    /// </summary>
    public class Configuration : DbConfiguration {
        public Configuration() {
            //DbInterception.Add(new EfInterceptor());
        }
    }
}