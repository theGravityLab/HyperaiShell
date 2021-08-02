using System;
using System.Collections.Generic;
using Ac682.Extensions.Logging.Console;
using Ac682.Extensions.Logging.Console.Formatters;
using Hangfire;
using Hangfire.Common;
using Hangfire.Storage.SQLite;
using Hyperai;
using Hyperai.Messages;
using Hyperai.Serialization;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Hangfire.Logging;
using HyperaiShell.App.Logging;
using HyperaiShell.App.Logging.ConsoleFormatters;
using HyperaiShell.App.Middlewares;
using HyperaiShell.App.Plugins;
using HyperaiShell.App.Services;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Plugins;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HyperaiShell.App
{
    public class Bootstrapper
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var cfgBuilder = new ConfigurationBuilder().AddTomlFile("appsettings.toml", false);
            var config = cfgBuilder.Build();

            var dbName = "data/internal.litedb.db";
            var database = new LiteDatabase(dbName);
            var repository = new LiteDbRepository(database);

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IRepository>(repository);


            services.AddLogging(options => options
                .AddConsole(c => c
                    .SetMinimalLevel(LogLevel.Information)
                    .AddBuiltinFormatters()
                    .AddFormatter<MessageElementFormatter>()
                    .AddFormatter<RelationFormatter>()
                    .AddFormatter<EventArgsFormatter>()
                )
                .AddFile("logs/app_{Date}.log")
            );

            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
            };
            //JsonConvert.DefaultSettings = () => settings;
            
            services.AddScoped(typeof(IPluginConfiguration<>), typeof(PluginConfiguration<>));
            services.AddScoped(typeof(IPluginRepository<>), typeof(PluginRepository<>));
            services.AddScoped<IMessageChainFormatter, HyperCodeFormatter>();
            services.AddScoped<IMessageChainParser, HyperCodeParser>();
            services.AddHangfire(configure => configure
                .UseLogProvider(new HangfireLogProvider())
                .UseSQLiteStorage("data/hangfire.sqlite.db")
                .UseSerializerSettings(settings));
            services.AddHangfireServer();
            services.AddDistributedMemoryCache();
            services.AddBots();
            services.AddClients(config);
            services.AddUnits();
            services.AddAttachments();
            services.AddAuthorizationService();
            services.AddBlacklist();

            services.AddHyperaiServer(options => options
                    .UseLogging()
                    .UseBlacklist()
                    .UseTranslator()
                    .UseBots()
                    .UseUnits());
        }
    }
}
