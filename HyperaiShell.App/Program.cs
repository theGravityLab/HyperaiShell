using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hyperai;
using Hyperai.Messages;
using Hyperai.Messages.ConcreteModels;
using Hyperai.Relations;
using Hyperai.Services;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.ModelExtensions;
using HyperaiShell.Foundation.Plugins;
using HyperaiShell.Foundation.Services;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace HyperaiShell.App
{
    public class Program
    {
        private static ILogger _logger;

        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            BsonMapper.Global = new BsonMapper(null, new AssemblyNameTypeNameBinder());

            var dirs = new[]
            {
                "plugins",
                "logs",
                "data",
                "config"
            }.Select(x => Path.Combine(Environment.CurrentDirectory, x));
            foreach (var dir in dirs)
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

            // var appBuilder = new HyperaiApplicationBuilder();
            //
            // appBuilder.UseStartup<Bootstrapper>();
            // LoadPackagesAsync().Wait();
            // SearchConfigurePluginServices(appBuilder);
            // Shared.Application = appBuilder.Build();
            // _logger = Shared.Application.Provider.GetRequiredService<ILogger<Program>>();
            // PrintAssemblyInfo();
            // ConfigurePlugins(Shared.Application);
            // Shared.Application.Run();

            var startup = new Bootstrapper();
            var hostBuilder = new HostBuilder()
                .ConfigureServices(startup.ConfigureServices)
                .UseConsoleLifetime();

            LoadPackagesAsync().Wait();
            SearchConfigurePluginServices(hostBuilder);
            Shared.Host = hostBuilder.Build();
            _logger = Shared.Host.Services.GetRequiredService<ILogger<Program>>();
            PrintAssemblyInfo();
            ConfigurePlugins(Shared.Host);
            var task = Shared.Host.RunAsync();
            //TEST HERE
            task.Wait();
        }

        private static void PrintAssemblyInfo()
        {
            _logger.LogInformation(@"
 ____            _ _   _                            _ 
|  _ \ _ __ ___ (_) | | |_   _ _ __   ___ _ __ __ _(_)
| |_) | '__/ _ \| | |_| | | | | '_ \ / _ \ '__/ _` | |
|  __/| | | (_) | |  _  | |_| | |_) |  __/ | | (_| | |
|_|   |_|  \___// |_| |_|\__, | .__/ \___|_|  \__,_|_|
              |__/       |___/|_|                     ");
            _logger.LogInformation("Powered by ProjHyperai\nHyperaiShell v{HyperaiShellV} (Plugin based on v{PluginBaseV})\nHyperai v{HyperaiV}\nHyperai.Units v{HyperaiUnitsV}",
                typeof(Program).Assembly.GetName().Version, 
                typeof(PluginBase).Assembly.GetName().Version,
                typeof(IApiClient).Assembly.GetName().Version,
                typeof(UnitService).Assembly.GetName().Version);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            if (e.IsTerminating)
            {
                _logger.LogCritical(exception, "Terminating for uncaught exception");
                Environment.ExitCode = -1;
            }
            else
            {
                _logger.LogError(exception, "Exception caught");
            }
        }

        /// <summary>
        ///     搜索插件并加载
        /// </summary>
        private static async Task LoadPackagesAsync()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "plugins"), "*.nupkg"))
                await PluginManager.Instance.LoadPackageAsync(file);
        }

        /// <summary>
        ///     使插件生效
        /// </summary>
        private static void SearchConfigurePluginServices(IHostBuilder builder)
        {
            var plugins = PluginManager.Instance.GetManagedPlugins();
            foreach (var type in plugins)
            {
                var plugin = PluginManager.Instance.Activate(type);
                builder.ConfigureServices(plugin.ConfigureServices);
            }
        }

        /// <summary>
        ///     初始化部分服务
        /// </summary>
        private static void ConfigurePlugins(IHost host)
        {
            host.Services.GetRequiredService<IUnitService>().SearchForUnits();
            var service = host.Services.GetRequiredService<IBotService>();
            var config = host.Services.GetRequiredService<IConfiguration>();
            foreach (var type in PluginManager.Instance.GetManagedPlugins())
            {
                var plugin = PluginManager.Instance.Activate(type);
                plugin.ConfigureBots(service.Builder, config);
                plugin.PostConfigure(config);
                _logger.LogInformation("Plugin {PluginIdentity} activated", plugin.Context.Meta.Identity);
            }
        }
    }
}
