using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HyperaiShell.App.Data;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Plugins;
using LiteDB;
using Microsoft.Extensions.Configuration;
using NuGet.Common;
using NuGet.Packaging;

namespace HyperaiShell.App.Plugins
{
    public class PluginManager
    {
        private readonly Dictionary<Type, PluginContext> plugins = new Dictionary<Type, PluginContext>();
        public static PluginManager Instance { get; } = new PluginManager();

        public IEnumerable<Type> GetManagedPlugins()
        {
            return plugins.Keys;
        }

        public PluginContext GetContextOfPlugin(Type plugin)
        {
            return plugins[plugin];
        }

        /// <summary>
        ///     从文件中加载程序集并获取其中派生自 <see cref="PluginBase" /> 的类型(存在多个则取第一个
        /// </summary>
        /// <param name="fileName">程序集文件</param>
        /// <returns>含有 <see cref="PluginBase" /> 派生类型的集合</returns>
        public Type Load(string fileName)
        {
            var ass = Assembly.LoadFrom(fileName);
            foreach (var type in ass.GetExportedTypes())
                if (!type.IsAbstract && type.IsSubclassOf(typeof(PluginBase)))
                    return type;
            return null;
        }

        /// <summary>
        ///     从文件中加载插件并管理(但不初始化)
        /// </summary>
        /// <param name="fileName">相对于运行目录的文件名</param>
        public async Task LoadPackageAsync(string fileName)
        {
            using var reader = new PackageArchiveReader(File.OpenRead(fileName), false);
            var nuspecReader = await reader.GetNuspecReaderAsync(CancellationToken.None);
            var identity = nuspecReader.GetId();
            var groups = await reader.GetLibItemsAsync(CancellationToken.None);
            var group = groups.FirstOrDefault();
            foreach (var packageFile in group!.Items)
            {
                if (!packageFile.EndsWith(".dll")) continue;

                var tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                var path = reader.ExtractFile(packageFile, tmpPath, NullLogger.Instance);
                //Type type = Load(await File.ReadAllBytesAsync(path));
                var type = Load(path);
                if (type == null) continue;
                var context = new PluginContext();
                var meta = new PluginMeta(identity, fileName,
                    Path.Combine(Environment.CurrentDirectory, "plugins", identity));
                context.Meta = meta;
                if (!Directory.Exists(meta.SpaceDirectory))
                {
                    Directory.CreateDirectory(meta.SpaceDirectory);
                    var item = (await reader.GetContentItemsAsync(CancellationToken.None)).FirstOrDefault();
                    if (item != null)
                        foreach (var file in item.Items)
                        {
                            if (file.EndsWith('/') || file.EndsWith('\\')) continue;

                            var entry = reader.GetEntry(file);
                            entry.SaveAsFile(Path.Combine(meta.SpaceDirectory, file.Substring(8)),
                                NullLogger.Instance);
                        }
                }

                var configFile = Path.Combine(meta.SpaceDirectory, "config.json");
                if (File.Exists(configFile))
                    context.Configuration = new Lazy<IConfiguration>(() =>
                    {
                        var builder = new ConfigurationBuilder();
                        builder.AddJsonFile(configFile);
                        return builder.Build();
                    });
                else
                    context.Configuration = new Lazy<IConfiguration>(() => new ConfigurationBuilder().Build());
                var dataFile = Path.Combine(meta.SpaceDirectory, "data.litedb.db");
                context.Repository = new Lazy<IRepository>(() => new LiteDbRepository(new LiteDatabase(dataFile)));
                plugins.Add(type, context);
            }
        }

        /// <summary>
        ///     构造一个插件实例并对其提供服务
        /// </summary>
        /// <param name="type">已注册的插件类型</param>
        public PluginBase Activate(Type type)
        {
            if (plugins.ContainsKey(type))
            {
                var plugin = (PluginBase) Activator.CreateInstance(type);
                plugin!.Context = plugins[type];
                return plugin;
            }

            throw new InvalidOperationException("Argument type for a plugin has not registered yet.");
        }
    }
}
