using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Hyperai.Services;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Packages;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Plugins;
using HyperaiShell.Foundation.Services;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using Sentry;
using PluginManager = HyperaiShell.App.Plugins.PluginManager;

namespace HyperaiShell.App
{
    public class Program
    {

        public async static Task Main()
        {
            // manager instance init
            PackageManager.Instance.PluginPackageLoaded = PluginPackageLoaded;
            
            //env init
            
            
            
            
            // search packages and load
            
            
            foreach (var nupkg in Directory.GetFiles("plugins/", "*.nupkg"))
            {
                await PackageManager.Instance.LoadPluginPackageAsync(nupkg);
            }
        }

        private static async void PluginPackageLoaded(string fileName,PackageArchiveReader reader, IEnumerable<Assembly> assemblies)
        {
            var plugins = assemblies.SelectMany(x =>
                x.GetExportedTypes().Where(y => !y.IsAbstract && y.IsSubclassOf(typeof(PluginBase))));
            var plugin = plugins.FirstOrDefault(); // 其他的都无视掉

            if (plugin != null)
            {
                var context = new PluginContext();
                var identity = await reader.GetIdentityAsync(CancellationToken.None);
                var meta = new PluginMeta(identity.Id, fileName,
                    Path.Combine("plugins", identity.Id));
                context.Meta = meta;
                if (!Directory.Exists(meta.SpaceDirectory))
                {
                    Directory.CreateDirectory(meta.SpaceDirectory);

                    var content = (await reader.GetContentItemsAsync(CancellationToken.None))
                        .OrderByDescending(x => x.TargetFramework.Version).FirstOrDefault();

                    if (content != null)
                    {
                        foreach (var item in content.Items)
                        {
                            await using var contentStream = await reader.GetStreamAsync(item, CancellationToken.None);
                            await using var fileStream = File.OpenWrite(Path.Combine(meta.SpaceDirectory,
                                item.Substring(item.IndexOf('/') + 1)));
                            await contentStream.CopyToAsync(fileStream);
                            await fileStream.FlushAsync();
                        }
                    }

                    var configFile = Path.Combine(meta.SpaceDirectory, "config.toml");
                    if (File.Exists(configFile))
                        context.Configuration =
                            new Lazy<IConfiguration>(() => new ConfigurationBuilder().AddTomlFile(configFile).Build());
                    else context.Configuration = new Lazy<IConfiguration>(() => new ConfigurationBuilder().Build());

                    var dataFile = Path.Combine(meta.SpaceDirectory, "data.lite.db");
                    context.Repository = new Lazy<IRepository>(() => new LiteDbRepository(new LiteDatabase(dataFile)));
                }
                
                PluginManager.Instance.RegisterPlugin(plugin, context);
            }
        }
    }
}
