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
    public class Bootstrapper : IHyperaiApplicationBuilderStartup
    {
        public void ConfigureMiddlewares(IHyperaiApplicationBuilder app)
        {
            app.UseLogging();
            app.UseBlacklist();
            app.UseTranslator();
            app.UseBots();
            app.UseUnits();
        }

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
                TypeNameHandling = TypeNameHandling.All
            };
            
            services.AddScoped(typeof(IPluginConfiguration<>), typeof(PluginConfiguration<>));
            services.AddScoped(typeof(IPluginRepository<>), typeof(PluginRepository<>));
            services.AddScoped<IMessageChainFormatter, HyperCodeFormatter>();
            services.AddScoped<IMessageChainParser, HyperCodeParser>();
            services.AddHangfire(configure => configure
                .UseColouredConsoleLogProvider()
                .UseSerializerSettings(settings)
                .UseDefaultActivator()
                .UseSQLiteStorage("data/hangfire.sqlite.db"));
            JobHelper.SetSerializerSettings(settings); //TODO: 这个 hangfire 说是会弃用该方法，实际上没有！对应方法反而无效
            services.AddDistributedMemoryCache();
            services.AddBots();
            services.AddClients(config);
            services.AddUnits();
            services.AddAttachments();
            services.AddAuthorization();
            services.AddBlacklist();
        }
    }
}
