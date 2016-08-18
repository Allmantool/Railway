using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;

namespace NaftanRailway.Domain.DbConfiguration {
    /// <summary>
    /// More flexible loggging for EF6 then Database.Log
    /// http://www.mortenanderson.net/logging-sql-statements-in-entity-framework-with-interception
    /// https://msdn.microsoft.com/en-us/data/jj680699.aspx
    /// </summary>
    public class EFInterceptor : IDbCommandInterceptor {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            Log(String.Format("NonQueryExecuted with the command {0}", command.CommandText));
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            Log(String.Format("NonQueryExecuting with the command {0}", command.CommandText));
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            Log(String.Format("ReaderExecuted with the command {0}", command.CommandText));
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            Log(String.Format("ReaderExecuting with the command {0}", command.CommandText));
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            Log(String.Format("ScalarExecuted with the command {0}", command.CommandText));
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            Log(String.Format("ScalarExecuting with the command {0}", command.CommandText));
        }
        /// <summary>
        /// Destination of log
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message) {
            Console.WriteLine(message);
        }
    }
}