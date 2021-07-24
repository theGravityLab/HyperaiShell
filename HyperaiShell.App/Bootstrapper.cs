using System;
using System.Collections.Generic;
using Ac682.Extensions.Logging.Console;
using Ac682.Extensions.Logging.Console.Formatters;
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
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

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
            var cfgBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
            var config = cfgBuilder.Build();

            var dbName = "data/internal.litedb.db";
            var database = new LiteDatabase(dbName);
            var repository = new LiteDbRepository(database);

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IRepository>(repository);

            var customThemeStyles =
                new Dictionary<ConsoleThemeStyle, SystemConsoleThemeStyle>
                {
                    {
                        ConsoleThemeStyle.Text, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.White
                        }
                    },
                    {
                        ConsoleThemeStyle.SecondaryText, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Gray
                        }
                    },
                    {
                        ConsoleThemeStyle.TertiaryText, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.DarkGray
                        }
                    },
                    {
                        ConsoleThemeStyle.LevelVerbose, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Gray
                        }
                    },
                    {
                        ConsoleThemeStyle.LevelDebug, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.DarkMagenta
                        }
                    },
                    {
                        ConsoleThemeStyle.LevelInformation, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Green
                        }
                    },
                    {
                        ConsoleThemeStyle.LevelWarning, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Yellow
                        }
                    },
                    {
                        ConsoleThemeStyle.LevelError, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Black,
                            Background = ConsoleColor.Red
                        }
                    },
                    {
                        ConsoleThemeStyle.LevelFatal, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.White,
                            Background = ConsoleColor.Red
                        }
                    },
                    {
                        ConsoleThemeStyle.String, new SystemConsoleThemeStyle
                        {
                            Foreground = ConsoleColor.Blue
                        }
                    },
                    {
                        ConsoleThemeStyle.Boolean, new SystemConsoleThemeStyle()
                        {
                            Foreground = ConsoleColor.Blue
                        }
                    },
                    {
                        ConsoleThemeStyle.Null, new SystemConsoleThemeStyle()
                        {
                            Foreground = ConsoleColor.Blue
                        }
                    },
                    {
                        ConsoleThemeStyle.Invalid, new SystemConsoleThemeStyle()
                        {
                            Foreground = ConsoleColor.DarkMagenta
                        }
                    },
                    {
                        ConsoleThemeStyle.Number, new SystemConsoleThemeStyle()
                        {
                            Foreground = ConsoleColor.DarkYellow
                        }
                    },
                    {
                        ConsoleThemeStyle.Scalar, new SystemConsoleThemeStyle()
                        {
                            Foreground = ConsoleColor.DarkYellow
                        }
                    },
                    {
                        ConsoleThemeStyle.Name, new SystemConsoleThemeStyle()
                        {
                            Foreground = ConsoleColor.DarkCyan
                        }
                    }
                };

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

            services.AddScoped(typeof(IPluginConfiguration<>), typeof(PluginConfiguration<>));
            services.AddScoped(typeof(IPluginRepository<>), typeof(PluginRepository<>));
            services.AddScoped<IMessageChainFormatter, HyperCodeFormatter>();
            services.AddScoped<IMessageChainParser, HyperCodeParser>();
            services.AddHangfire();
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
