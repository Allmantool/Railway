using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Railway.DeliveryCargo.Data.EF.Factories
{
    public sealed class LoggerFactorySingleton
    {
        [Obsolete]
        private static readonly Lazy<LoggerFactory> _instance = new Lazy<LoggerFactory>(
            () => new LoggerFactory(
                new[] {
                    new DebugLoggerProvider(
                        (category, level) =>
                            category == DbLoggerCategory.Database.Command.Name
                            && level == LogLevel.Information),
                }
            ));

        [Obsolete]
        public static LoggerFactory Instance => _instance.Value;
    }
}
