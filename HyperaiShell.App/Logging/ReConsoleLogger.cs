using Microsoft.Extensions.Logging;
using System;

namespace HyperaiShell.App.Logging
{
    public class ReConsoleLogger : ILogger
    {
        private readonly string _name;
        private readonly LogLevel _minmalLevel;

        public ReConsoleLogger(string name, LogLevel minimalLevel = LogLevel.Debug)
        {
            _name = name;
            _minmalLevel = minimalLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (int)logLevel >= (int)_minmalLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string levelName = logLevel switch
            {
                LogLevel.Trace => "TRAC",
                LogLevel.Debug => "DEBG",
                LogLevel.Information => "INFO",
                LogLevel.Warning => "WARN",
                LogLevel.Error => "ERRO",
                LogLevel.Critical => "CRIT",
                _ => "NONE",
            };
            // [20/07/22 00:11][DEBG]NAME => STHSTH
            string datetime = DateTime.Now.ToString("[yy/MM/dd HH:mm:ss] ");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(datetime);
            (ConsoleColor, ConsoleColor) color = GetColor(logLevel);
            Console.BackgroundColor = color.Item1;
            Console.ForegroundColor = color.Item2;
            Console.Write($"[{levelName}]");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($" {_name}({eventId.Id}) =>");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(formatter(state, exception));
        }

        private (ConsoleColor, ConsoleColor) GetColor(LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => (ConsoleColor.Black, ConsoleColor.Cyan),
                LogLevel.Debug => (ConsoleColor.Black, ConsoleColor.DarkMagenta),
                LogLevel.Information => (ConsoleColor.Black, ConsoleColor.Green),
                LogLevel.Warning => (ConsoleColor.Black, ConsoleColor.Yellow),
                LogLevel.Error => (ConsoleColor.Black, ConsoleColor.Red),
                LogLevel.Critical => (ConsoleColor.Red, ConsoleColor.White),
                _ => (ConsoleColor.Black, ConsoleColor.Gray),
            };
        }
    }
}