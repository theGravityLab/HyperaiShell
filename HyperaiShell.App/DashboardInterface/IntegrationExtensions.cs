using System.Threading;
using HyperaiShell.App.DashboardInterface.Scenarios;
using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;
using static Terminal.Gui.TabView;

namespace HyperaiShell.App.DashboardInterface
{
    public static class IntegrationExtensions
    {
        public static IServiceCollection AddDashboardIntegration(this IServiceCollection services)
        {
            
            return services
            .AddScenario<StatusScenario>()
            .AddScenario<LogScenario>();
        }

        public static IServiceCollection AddScenario<TScenario>(this IServiceCollection services)
        where TScenario: ScenarioBase
        {
            return services.AddTransient<ScenarioBase,TScenario>();
        }
    }
}
