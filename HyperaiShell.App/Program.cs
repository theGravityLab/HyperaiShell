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
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HyperaiShell.App
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            BsonMapper.Global = new BsonMapper(null, new ToStringTypeNameBinder());

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
            MakeItWork(Shared.Application);
            Shared.Application.Run();
            // TODO: uncomment it to run the application
        }

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Shared.Application.StopAsync().Wait();
        }

        /// <summary>
        /// 搜索插件并加载
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        private static async Task FuckUnitTestButMyGuidelineTellMeItIsRequiredInHugeProjectsSoHaveToKeepItBYWSomeTestsMayNotWorkAndMissing(IHyperaiApplicationBuilder app)
        {
            //PluginManager.Instance.Init(app.Services);

            // search for nupkg files and load then
            foreach (string file in Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "plugins"), "*.nupkg"))
            {
                await PluginManager.Instance.LoadAsync(file);
            }
        }

        /// <summary>
        /// 使插件生效
        /// </summary>
        private static void NothingToSay(IHyperaiApplicationBuilder app)
        {
            System.Collections.Generic.IEnumerable<Type> plugins = PluginManager.Instance.GetManagedPlugins();
            foreach (Type type in plugins)
            {
                PluginBase plugin = (PluginBase)Activator.CreateInstance(type);
                plugin.ConfigureServices(app.Services);
            }
        }

        /// <summary>
        /// 初始化部分服务
        /// </summary>
        private static void MakeItWork(IHyperaiApplication app)
        {
            app.Provider.GetRequiredService<IUnitService>().SearchForUnits();
            IBotService service = app.Provider.GetRequiredService<IBotService>();
            foreach (Type type in PluginManager.Instance.GetManagedPlugins())
            {
                PluginBase plugin = (PluginBase)Activator.CreateInstance(type);
                plugin.ConfigureBots(service.Builder);
            }
        }

    }
}
