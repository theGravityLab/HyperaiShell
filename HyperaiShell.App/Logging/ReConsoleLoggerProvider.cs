using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public class ReConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> loggers = new ConcurrentDictionary<string, ILogger>();

        private readonly ReConsoleLoggerOptions _options;
        public ReConsoleLoggerProvider(ReConsoleLoggerOptions options)
        {
            _options = options;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, (name) => new ReConsoleLogger(categoryName, _options.MinmalLevel));
        }

        public void Dispose()
        {
            loggers.Clear();
        }
    }
}