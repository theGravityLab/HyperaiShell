using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public static class ReConsoleExtensions
    {
        public static ILoggingBuilder AddReConsole(this ILoggingBuilder builder)
        {
            return AddReConsole(builder, new ReConsoleLoggerOptions() { MinmalLevel = LogLevel.Debug });
        }

        public static ILoggingBuilder AddReConsole(this ILoggingBuilder builder, ReConsoleLoggerOptions options)
        {
            return builder.AddProvider(new ReConsoleLoggerProvider(options));
        }
    }
}