using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hyperai;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.Services;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App
{
    public static class Program
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

            var app = new HyperaiApplicationBuilder();

            app.UseStartup<Bootstrapper>();
            LoadPackages().Wait();
            SearchConfigurePluginServices(app);
            Shared.Application = app.Build();
            _logger = Shared.Application.Provider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
            ConfigurePlugins(Shared.Application);
            Shared.Application.Run();
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
                _logger.LogCritical(exception, "Terminating for exception uncaught.");
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
            // NOTE: search all units here
            app.Provider.GetRequiredService<IUnitService>().SearchForUnits();
            var service = app.Provider.GetRequiredService<IBotService>();
            var config = app.Provider.GetRequiredService<IConfiguration>();
            foreach (var type in PluginManager.Instance.GetManagedPlugins())
            {
                var plugin = PluginManager.Instance.Activate(type);
                plugin.ConfigureBots(service.Builder, config);
                plugin.PostConfigure(config);
                _logger.LogInformation("Plugin ({}) activated.", plugin.Context.Meta.Identity);
            }
        }
    }
}