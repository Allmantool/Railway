namespace NaftanRailway.Domain.Services.DbConfigurations {
    using System;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Diagnostics;

    public class EfInterceptor : IDbCommandInterceptor {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this.Log(string.Format(
                "NonQueryExecuted with the command:{0}{1}",
                Environment.NewLine,
                command.CommandText));
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this.Log(string.Format("NonQueryExecuting with the command:{0}{1}", Environment.NewLine, command.CommandText));
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this.Log(string.Format("ReaderExecuted with the command:{0}{1}", Environment.NewLine, command.CommandText));
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            this.Log(string.Format("ReaderExecuting with the command:{0}{1}", Environment.NewLine, command.CommandText));
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this.Log(string.Format("ScalarExecuted with the command:{0}{1}", Environment.NewLine, command.CommandText));
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            this.Log(string.Format("ScalarExecuting with the command:{0}{1}", Environment.NewLine, command.CommandText));
        }

        private void Log(string message) {
            Debug.WriteLine(message);
        }
    }
}