namespace NaftanRailway.Domain.Services.DbConfigurations {
    using System;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure.Interception;
    using System.Diagnostics;

    /// <summary>
    ///     More flexible logging for EF6 then Database.Log
    ///     http://www.mortenanderson.net/logging-sql-statements-in-entity-framework-with-interception
    ///     https://msdn.microsoft.com/en-us/data/jj680699.aspx
    /// </summary>
    public class EfInterceptor : IDbCommandInterceptor {
        public void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext) {
            this.Log(string.Format("NonQueryExecuted with the command:{0}{1}", Environment.NewLine, command.CommandText));
        }

        /// <summary>
        ///     ExecuteNonQuery used for executing queries that does not return any data.
        ///     It is used to execute the sql statements like update, insert, delete etc.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="interceptionContext"></param>
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

        /// <summary>
        ///     Destination of log
        ///     Console.WriteLine writes to the standard output stream, either in debug or release.
        ///     Debug.WriteLine writes to the trace listeners in the Listeners collection, but only when running in debug.
        ///     When the application is compiled in the release configuration, the Debug elements will not be compiled into the
        ///     code.
        ///     As Debug.WriteLine writes to all the trace listeners in the Listeners collection,
        ///     it is possible that this could be output in more than one place (Visual Studio output window, Console, Log file,
        ///     third-party application
        ///     which registers a listener (I believe DebugView does this), etc.).
        ///     Debug and Trace both write out to the same location, the Listeners collection. By default it is routed to Visual
        ///     Studio's Debug window, however you can put code in your app.config file to redirect it to other locations when you
        ///     are not debugging.
        ///     The difference between Debug and Trace is all of the methods in Debug only write out when the DEBUG compilation
        ///     symbol is set(default on for debug,
        ///     off for release) when the symbol is not set the methods are never called in your code.Trace looks for the TRACE
        ///     symbol (default on for both debug and release).
        ///     Other that, the two classes are identical.In fact if you modify Debug.Listeners to add a new listener it will
        ///     also modify Trace.Listeners as both just point to
        ///     the internal static property TraceInternal.Listeners
        ///     As for picking which one to use, Do you want diagnostic information to show up in release and debug mode? use
        ///     Trace, Debug only? use Debug.
        ///     Do you want it to be visible to a end user without a debugger attached? use Console or add a console trace
        ///     listener.
        /// </summary>
        /// <param name="message"></param>
        private void Log(string message) {
            /*log for EF6 dbcontext in output window (debug mode)*/
            Debug.WriteLine(message);
            //    Trace.Write(message);
        }
    }
}