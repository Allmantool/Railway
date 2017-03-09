using System.Data.Entity;

namespace NaftanRailway.Domain.DbConfigurations
{
    /// <summary>
    ///     More flexible logging for EF6 then Database.Log
    ///     !!!Warning!!!! It's possible Conflict with registration in web.config or body code (double log)
    ///     http://www.mortenanderson.net/logging-sql-statements-in-entity-framework-with-interception
    ///     https://msdn.microsoft.com/en-us/data/jj680699.aspx
    /// </summary>
    public class Configuration : DbConfiguration
    {
    }
}