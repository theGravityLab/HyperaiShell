using HyperaiShell.Foundation.Bots;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HyperaiShell.App.Bots
{
    public class BotBuilder : IBotBuilder
    {
        private readonly Type _botType;
        private Action<BotBase> _configure = null;

        public BotBuilder(Type botType)
        {
            _botType = botType;
        }

        public BotBase Build(IServiceProvider provider)
        {
            BotBase bot = (BotBase)(ActivatorUtilities.CreateInstance(provider, _botType));
            if (_configure != null)
            {
                _configure(bot);
            }

            return bot;
        }

        public IBotBuilder Configure(Action<BotBase> configure)
        {
            _configure = configure;
            return this;
        }
    }
}