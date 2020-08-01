using Hyperai.Services;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace HyperaiShell.App.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
        {
            string profileName = configuration["Application:SelectedClientName"];
            IConfigurationSection profile = configuration.GetSection("Clients").GetChildren().Where(x => x["Name"] == profileName).First();
            Type clientType = null;
            Type optionsType = null;
            clientType = Type.GetType(profile["ClientTypeDefined"], false);
            optionsType = Type.GetType(profile["OptionsTypeDefined"], true);
            IConfigurationSection optionsSection = profile.GetSection("Options");
            services.AddSingleton(typeof(IApiClient), clientType);
            services.AddSingleton(optionsType, optionsSection.Get(optionsType));

            return services;
        }

        public static IServiceCollection AddBots(this IServiceCollection services)
        {
            services.AddSingleton<IBotService, BotService>();
            return services;
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationService, AuthorizationService>();
            return services;
        }

        public static IServiceCollection AddAttachments(this IServiceCollection services)
        {
            services.AddSingleton<IAttachmentService, AttachmentService>();
            return services;
        }

        public static IServiceCollection AddBlacklist(this IServiceCollection services)
        {
            services.AddSingleton<IBlockService, BlockService>();
            return services;
        }
    }
}