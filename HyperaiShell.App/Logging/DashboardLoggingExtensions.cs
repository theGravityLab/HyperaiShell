using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Logging
{
    public static class DashboardLoggingExtensions
    {
        public static ILoggingBuilder AddDashboardLogger(this ILoggingBuilder builder)
        {
            return builder.AddProvider(new DashboardLoggerProvider());
        }
    }
}
