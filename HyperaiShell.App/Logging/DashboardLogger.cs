using System;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public class DashboardLogger : ILogger
    {
        private readonly string _name;

        public DashboardLogger(string name)
        {
            _name = name;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel > LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            DashboardLoggingStore.Instance.Log(logLevel, formatter(state, exception), _name, exception);
        }
    }
}
