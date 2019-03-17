namespace Railway.Domain.Services.DbConfigurations {
    using System;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Diagnostics;

    public class EfInterceptor : IDbCommandInterceptor {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            Log($"NonQueryExecuted with the command:{Environment.NewLine}{command.CommandText}");
        }

        public void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            Log($"NonQueryExecuting with the command:{Environment.NewLine}{command.CommandText}");
        }

        public void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            Log($"ReaderExecuted with the command:{Environment.NewLine}{command.CommandText}");
        }

        public void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext) {
            Log($"ReaderExecuting with the command:{Environment.NewLine}{command.CommandText}");
        }

        public void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            Log($"ScalarExecuted with the command:{Environment.NewLine}{command.CommandText}");
        }

        public void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext) {
            Log($"ScalarExecuting with the command:{Environment.NewLine}{command.CommandText}");
        }

        private void Log(string message) {
            Debug.WriteLine(message);
        }
    }
}