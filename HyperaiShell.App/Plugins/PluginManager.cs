using HyperaiShell.App.Data;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Plugins;
using LiteDB;
using Microsoft.Extensions.Configuration;
using NuGet.Common;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HyperaiShell.App.Plugins
{
    public class PluginManager
    {
        public static PluginManager Instance { get; private set; } = new PluginManager();

        private readonly Dictionary<Type, PluginContext> plugins = new Dictionary<Type, PluginContext>();

        public IEnumerable<Type> GetManagedPlugins()
        {
            return plugins.Keys;
        }

        public PluginContext GetContextOfPlugin(Type plugin)
        {
            return plugins[plugin];
        }

        /// <summary>
        /// 从文件中加载程序集并获取其中派生自 <see cref="PluginBase" /> 的类型(存在多个则取第一个
        /// </summary>
        /// <param name="fileName">程序集文件</param>
        /// <returns>含有 <see cref="PluginBase" /> 派生类型的集合</returns>
        public Type Load(string fileName)
        {
            Assembly ass = Assembly.LoadFrom(fileName);
            foreach (Type type in ass.GetExportedTypes())
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(PluginBase)))
                {
                    return type;
                }
            }
            return null;
        }

        /// <summary>
        /// 从文件中加载插件并管理(但不初始化)
        /// </summary>
        /// <param name="fileName">相对于运行目录的文件名</param>
        public async Task LoadPackageAsync(string fileName)
        {
            using PackageArchiveReader reader = new PackageArchiveReader(File.OpenRead(fileName), false);
            NuspecReader nuspecReader = await reader.GetNuspecReaderAsync(CancellationToken.None);
            string identity = nuspecReader.GetId();
            IEnumerable<FrameworkSpecificGroup> groups = await reader.GetLibItemsAsync(CancellationToken.None);
            FrameworkSpecificGroup group = groups.Where(x => x.TargetFramework.GetShortFolderName().StartsWith("netstandard")).OrderByDescending(x => x.TargetFramework.GetShortFolderName()).FirstOrDefault();
            foreach (string packageFile in group.Items)
            {
                if (!packageFile.EndsWith(".dll"))
                {
                    continue;
                }

                string tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                string path = reader.ExtractFile(packageFile, tmpPath, NullLogger.Instance);
                //Type type = Load(await File.ReadAllBytesAsync(path));
                Type type = Load(path);
                if (type != null)// 为null时则不是插件程序集， 但不一定不是依赖程序集
                {
                    PluginContext context = new PluginContext();
                    PluginMeta meta = new PluginMeta(identity, fileName, Path.Combine(Environment.CurrentDirectory, "plugins", identity));
                    context.Meta = meta;
                    if (!Directory.Exists(meta.SpaceDirectory))
                    {
                        Directory.CreateDirectory(meta.SpaceDirectory);
                        FrameworkSpecificGroup item = (await reader.GetContentItemsAsync(CancellationToken.None)).FirstOrDefault();
                        if (item != null)
                        {
                            foreach (string file in item.Items)
                            {
                                if (file.EndsWith('/') || file.EndsWith('\\'))
                                {
                                    continue;
                                }

                                ZipArchiveEntry entry = reader.GetEntry(file);
                                entry.SaveAsFile(Path.Combine(meta.SpaceDirectory, file.Substring(8)), NullLogger.Instance);
                            }
                        }
                    }
                    string configFile = Path.Combine(meta.SpaceDirectory, "config.json");
                    if (File.Exists(configFile))
                    {
                        context.Configuration = new Lazy<IConfiguration>(() =>
                        {
                            ConfigurationBuilder builder = new ConfigurationBuilder();
                            builder.AddJsonFile(configFile);
                            return builder.Build();
                        });
                    }
                    else
                    {
                        context.Configuration = new Lazy<IConfiguration>(() => (new ConfigurationBuilder().Build()));
                    }
                    string dataFile = Path.Combine(meta.SpaceDirectory, "data.db");
                    context.Repository = new Lazy<IRepository>(() => new LiteDbRepository(new LiteDatabase(dataFile)));
                    plugins.Add(type, context);
                }
            }
        }

        /// <summary>
        /// 构造一个插件实例并对其提供服务
        /// </summary>
        /// <param name="type">已注册的插件类型</param>
        public PluginBase Activate(Type type)
        {
            if (plugins.ContainsKey(type))
            {
                PluginBase plugin = (PluginBase)Activator.CreateInstance(type);
                plugin.Context = plugins[type];
                return plugin;
            }
            else
            {
                throw new InvalidOperationException("Argument type for a plugin has not regeristered yet.");
            }
        }
    }
}