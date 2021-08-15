using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public class DashboardLoggingStore
    {
        public static DashboardLoggingStore Instance => _Instance ??= new DashboardLoggingStore();
        private static DashboardLoggingStore _Instance;
        public List<LogItem> Logs {get; private set;} = new List<LogItem>();

        public void Log(LogLevel level, string message, string source, Exception exception)
        {
            Logs.Add(new LogItem()
            {
                Level = level,
                Time = DateTime.Now,
                Source = source,
                Message = message,
                Exception = exception
            });
            if(Logs.Count > 128)
            {
                Logs.RemoveAt(Logs.Count - 1);
            }
        }
    }
}
