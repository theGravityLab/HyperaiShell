using Hyperai;
using Hyperai.Messages;
using Hyperai.Serialization;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Logging;
using HyperaiShell.App.Middlewares;
using HyperaiShell.App.Plugins;
using HyperaiShell.App.Services;
using HyperaiShell.Foundation.Data;
using HyperaiShell.Foundation.Plugins;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
            IConfigurationBuilder cfgBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
            IConfigurationRoot config = cfgBuilder.Build();

            LiteDatabase database = new LiteDatabase("data/internal.db");
            LiteDbRepository repository = new LiteDbRepository(database);

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IRepository>(repository);

            services.AddLogging(options => options
                .AddConfiguration(config)
                .AddDebug()
                .AddReConsole()
                .AddFile("logs/app_{Date}.log", minimumLevel: LogLevel.Information)
                .SetMinimumLevel(LogLevel.Trace)
                );

            services.AddScoped(typeof(IPluginConfiguration<>), typeof(PluginConfiguration<>));
            services.AddScoped(typeof(IPluginRepository<>), typeof(PluginRepository<>));
            services.AddScoped<IMessageChainFormatter, HyperCodeFormatter>();
            services.AddScoped<IMessageChainParser, HyperCodeParser>();
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