using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public class ReConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ReConsoleLoggerOptions _options;
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        public ReConsoleLoggerProvider(ReConsoleLoggerOptions options)
        {
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, name => new ReConsoleLogger(categoryName, _options.MinmalLevel));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}