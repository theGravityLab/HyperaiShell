using Hyperai.Events;
using Hyperai.Relations;
using Hyperai.Services;
using HyperaiShell.App.Bots;
using HyperaiShell.Foundation.Bots;
using HyperaiShell.Foundation.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperaiShell.App.Services
{
    public class BotService : IBotService
    {
        private BotCollection bots;

        private readonly IApiClient _client;
        private readonly IServiceProvider _provider;

        public IBotCollectionBuilder Builder { get; private set; } = new BotCollectionBuilder();

        public BotService(IApiClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        public async Task PushAsync(GenericEventArgs args)
        {
            Self self = await _client.RequestAsync<Self>(null);
            switch (args)
            {
                case FriendMessageEventArgs friendMessage:
                    DoForAll(x => x.OnFriendMessage(_client, friendMessage), self);
                    break;

                case GroupMessageEventArgs groupMessage:
                    DoForAll(x => x.OnGroupMessage(_client, groupMessage), self);
                    break;

                case FriendRecallEventArgs friendRecall:
                    DoForAll(x => x.OnFriendRecall(_client, friendRecall), self);
                    break;

                case GroupRecallEventArgs groupRecall:
                    DoForAll(x => x.OnGroupRecall(_client, groupRecall), self);
                    break;

                default:
                    break;
            }
        }

        private void DoForAll(Action<BotBase> action, Self me)
        {
            if (bots == null)
            {
                bots = Builder.Build(_provider);
            }

            foreach (BotBase bot in bots)
            {
                bot.Me = me;
                Task.Run(() => action(bot), CancellationToken.None);
            }
        }
    }
}