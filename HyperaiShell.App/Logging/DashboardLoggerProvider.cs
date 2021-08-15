using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public class DashboardLoggerProvider: ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> loggers = new();

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName,
                name => new DashboardLogger(name));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}
