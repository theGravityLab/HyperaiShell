using Ac682.Extensions.Logging.Console;
using Hangfire;
using Hangfire.Storage.SQLite;
using Hyperai.Messages;
using Hyperai.Serialization;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Hangfire.Logging;
using HyperaiShell.App.Logging.ConsoleFormatters;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Plugins;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using HyperaiShell.App.Services;
using Sentry.Extensions.Logging.Extensions.DependencyInjection;
using HyperaiShell.App.Logging;
using Hyperai;
using HyperaiShell.App.Middlewares;

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

            services.AddLogging(builder =>
            {
                builder
                    .AddConfiguration(config)
                    .AddDebug()
                    .AddFile("logs/app_{date}.log");

                if (config["Application:SentryEnabled"]?.ToUpper() == "TRUE") builder.AddSentry();
                
                else
                {
                    builder.AddConsole(c => c
                        .SetMinimalLevel(LogLevel.Debug)
                        .AddBuiltinFormatters()
                        .AddFormatter<MessageElementFormatter>()
                        .AddFormatter<RelationFormatter>()
                        .AddFormatter<EventArgsFormatter>()
                    );
                }
            });

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All
            };

            services.AddScoped(typeof(IPluginConfiguration<>), typeof(PluginConfiguration<>))
                .AddScoped(typeof(IPluginRepository<>), typeof(PluginRepository<>))
                .AddScoped<IMessageChainFormatter, HyperCodeFormatter>()
                .AddScoped<IMessageChainParser, HyperCodeParser>()
                .AddHangfire(configure => configure
                    .UseLogProvider(new HangfireLogProvider())
                    .UseSQLiteStorage("data/hangfire.sqlite.db")
                    .UseSerializerSettings(settings))
                .AddHttpClient()
                .AddHangfireServer()
                .AddHyperaiServer(options => options
                    .UseLogging()
                    .UseBlacklist()
                    .UseTranslator()
                    .UseBots()
                    .UseUnits())
                .AddDistributedMemoryCache()
                .AddBots()
                .AddClients(config)
                .AddUnits()
                .AddAttachments()
                .AddAuthorizationService()
                .AddBlacklist();
        }
    }
}
