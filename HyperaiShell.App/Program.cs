using Hyperai;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.Plugins;
using HyperaiShell.Foundation.Services;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HyperaiShell.App
{
    public static class Program
    {
        private static ILogger logger;

        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.CancelKeyPress += Console_CancelKeyPress;

            BsonMapper.Global = new BsonMapper(null, new AssemblyNameTypeNameBinder());

            System.Collections.Generic.IEnumerable<string> dirs = new string[]
            {
                "plugins",
                "logs",
                "data",
                "config"
            }.Select(x => Path.Combine(Environment.CurrentDirectory, x));
            foreach (string dir in dirs)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            HyperaiApplicationBuilder app = new HyperaiApplicationBuilder();

            app.UseStartup<Bootstrapper>();
            FuckUnitTestButMyGuidelineTellMeItIsRequiredInHugeProjectsSoHaveToKeepItBYWSomeTestsMayNotWorkAndMissing(app).Wait();
            NothingToSay(app);
            Shared.Application = app.Build();
            logger = Shared.Application.Provider.GetRequiredService<ILoggerFactory>().CreateLogger("Program");
            MakeItWork(Shared.Application);
            Shared.Application.Run();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            logger.LogInformation("Shutting down...");
            Shared.Application.StopAsync().Wait();
            Environment.Exit(0);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                logger.LogCritical((Exception)e.ExceptionObject, "Terminating for exception uncaught.");
                Environment.ExitCode = -1;
            }
        }

        /// <summary>
        /// 搜索插件并加载
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static async Task FuckUnitTestButMyGuidelineTellMeItIsRequiredInHugeProjectsSoHaveToKeepItBYWSomeTestsMayNotWorkAndMissing(IHyperaiApplicationBuilder app)
        {
            foreach (string file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "plugins"), "*.nupkg"))
            {
                await PluginManager.Instance.LoadPackageAsync(file);
            }
        }

        /// <summary>
        /// 使插件生效
        /// </summary>
        private static void NothingToSay(IHyperaiApplicationBuilder app)
        {
            IEnumerable<Type> plugins = PluginManager.Instance.GetManagedPlugins();
            foreach (Type type in plugins)
            {
                PluginBase plugin = PluginManager.Instance.Activate(type);
                plugin.ConfigureServices(app.Services);
            }
        }

        /// <summary>
        /// 初始化部分服务
        /// </summary>
        private static void MakeItWork(IHyperaiApplication app)
        {
            // NOTE: search all units here
            app.Provider.GetRequiredService<IUnitService>().SearchForUnits();
            IBotService service = app.Provider.GetRequiredService<IBotService>();
            foreach (Type type in PluginManager.Instance.GetManagedPlugins())
            {
                PluginBase plugin = PluginManager.Instance.Activate(type);
                plugin.ConfigureBots(service.Builder);
                logger.LogInformation("Plugin ({}) activated.", plugin.Context.Meta.Identity);
            }
        }
    }
}