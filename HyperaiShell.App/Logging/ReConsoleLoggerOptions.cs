using System;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    
    [Obsolete]
    public class ReConsoleLoggerOptions
    {
        public LogLevel MinmalLevel { get; set; } = LogLevel.Debug;
    }
}
