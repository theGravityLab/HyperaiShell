using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperai;
using Hyperai.Services;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.Plugins;
using HyperaiShell.Foundation.Services;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            Console.CancelKeyPress += Console_CancelKeyPress;

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

            var appBuilder = new HyperaiApplicationBuilder();

            appBuilder.UseStartup<Bootstrapper>();
            LoadPackages().Wait();
            SearchConfigurePluginServices(appBuilder);
            Shared.Application = appBuilder.Build();
            _logger = Shared.Application.Provider.GetRequiredService<ILogger<Program>>();
            PrintAssemblyInfo();
            ConfigurePlugins(Shared.Application);
            Shared.Application.Run();
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

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _logger.LogInformation("Shutting down...");
            Shared.Application.StopAsync().Wait();
            Environment.Exit(0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            if (e.IsTerminating)
            {
                _logger.LogCritical(exception, "Terminating for uncaught exception.");
                Environment.ExitCode = -1;
            }
            else
            {
                _logger.LogError(exception, "Exception caught.");
            }
        }

        /// <summary>
        ///     搜索插件并加载
        /// </summary>
        private static async Task LoadPackages()
        {
            foreach (var file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "plugins"), "*.nupkg"))
                await PluginManager.Instance.LoadPackageAsync(file);
        }

        /// <summary>
        ///     使插件生效
        /// </summary>
        private static void SearchConfigurePluginServices(IHyperaiApplicationBuilder app)
        {
            var plugins = PluginManager.Instance.GetManagedPlugins();
            foreach (var type in plugins)
            {
                var plugin = PluginManager.Instance.Activate(type);
                plugin.ConfigureServices(app.Services);
            }
        }

        /// <summary>
        ///     初始化部分服务
        /// </summary>
        private static void ConfigurePlugins(IHyperaiApplication app)
        {
            app.Provider.GetRequiredService<IUnitService>().SearchForUnits();
            var service = app.Provider.GetRequiredService<IBotService>();
            var config = app.Provider.GetRequiredService<IConfiguration>();
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
