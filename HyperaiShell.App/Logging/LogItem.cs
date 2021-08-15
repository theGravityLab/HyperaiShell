using System;
using Hangfire.Storage.SQLite;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public struct LogItem
    {
        public string Message;
        public DateTime Time;
        public string Source;
        public LogLevel Level;
        public Exception Exception;

        public override string ToString() => string.Format("{0} {1}", Level.ToString()[0], Message);
    }
}
