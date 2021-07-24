using System;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    
    [Obsolete]
    public class ReConsoleLoggerOptions
    {
        public LogLevel MinimalLevel { get; set; } = LogLevel.Debug;
    }
}
