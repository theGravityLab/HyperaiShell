using System;
using System.Threading;
using System.Threading.Tasks;
using Hyperai.Events;
using Hyperai.Relations;
using Hyperai.Services;
using HyperaiShell.App.Bots;
using HyperaiShell.Foundation.Bots;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.Logging;
using Sentry;

namespace HyperaiShell.App.Services
{
    public class BotService : IBotService
    {
        private readonly IApiClient _client;
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        private readonly IHub _hub;
        private BotCollection bots;

        public BotService(IApiClient client, IServiceProvider provider, ILogger<BotService> logger, IHub hub)
        {
            _client = client;
            _provider = provider;
            _logger = logger;
            _hub = hub;
        }

        public IBotCollectionBuilder Builder { get; } = new BotCollectionBuilder();

        public async Task PushAsync(GenericEventArgs args)
        {
            var transaction = _hub.StartTransaction(nameof(HyperaiShell), $"{nameof(BotService)}-{nameof(PushAsync)}",
                args.GetType().Name);
            var self = await _client.RequestAsync<Self>(null);
            switch (args)
            {
                case FriendMessageEventArgs friendMessage:
                    await DoForAllAsync(x => x.OnFriendMessage(_client, friendMessage), self);
                    break;

                case GroupMessageEventArgs groupMessage:
                    await DoForAllAsync(x => x.OnGroupMessage(_client, groupMessage), self);
                    break;

                case FriendRecallEventArgs friendRecall:
                    await DoForAllAsync(x => x.OnFriendRecall(_client, friendRecall), self);
                    break;

                case GroupRecallEventArgs groupRecall:
                    await DoForAllAsync(x => x.OnGroupRecall(_client, groupRecall), self);
                    break;

                case GroupMemberRequestEventArgs groupMemberRequest:
                    await DoForAllAsync(x => x.OnMemberRequest(_client, groupMemberRequest), self);
                    break;

                case FriendRequestResponsedEventArgs friendRequestResp:
                    await DoForAllAsync(x => x.OnFriendRequestResp(_client, friendRequestResp), self);
                    break;

                case MemberRequestResponsedEventArgs memberRequestResp:
                    await DoForAllAsync(x => x.OnMemberRequestResp(_client, memberRequestResp), self);
                    break;

                case GroupJoinedEventArgs groupJoined:
                    await DoForAllAsync(x => x.OnGroupJoined(_client, groupJoined), self);
                    break;

                case GroupMemberJoinedEventArgs groupMemberJoined:
                    await DoForAllAsync(x => x.OnGroupMemberJoined(_client, groupMemberJoined), self);
                    break;

                case GroupMemberLeftEventArgs groupMemberLeft:
                    await DoForAllAsync(x => x.OnGroupMemberLeft(_client, groupMemberLeft), self);
                    break;

                case GroupLeftEventArgs groupLeft:
                    await DoForAllAsync(x => x.OnGroupLeft(_client, groupLeft), self);
                    break;

                case GroupMemberCardChangedEventArgs groupMemberCardChanged:
                    await DoForAllAsync(x => x.OnGroupMemberCardChanged(_client, groupMemberCardChanged), self);
                    break;

                case GroupMemberTitleChangedEventArgs groupMemberTitleChanged:
                    await DoForAllAsync(x => x.OnGroupMemberTitleChanged(_client, groupMemberTitleChanged), self);
                    break;

                case GroupMutedEventArgs groupMuted:
                    await DoForAllAsync(x => x.OnGroupMuted(_client, groupMuted), self);
                    break;

                case GroupNameChangedEventArgs groupNameChanged:
                    await DoForAllAsync(x => x.OnGroupNameChanged(_client, groupNameChanged), self);
                    break;

                case GroupPermissionChangedEventArgs groupPermissionChanged:
                    await DoForAllAsync(x => x.OnGroupPermissionChanged(_client, groupPermissionChanged), self);
                    break;

                case GroupUnmutedEventArgs groupUnmuted:
                    await DoForAllAsync(x => x.OnGroupUnmuted(_client, groupUnmuted), self);
                    break;

                case InvitationResponsedEventArgs invitationResp:
                    await DoForAllAsync(x => x.OnInvitationResp(_client, invitationResp), self);
                    break;
            }

            await DoForAllAsync(x => x.OnEverything(_client, args), self);
            transaction.Finish();
        }

        private async Task DoForAllAsync(Action<BotBase> action, Self me)
        {
            bots ??= Builder.Build(_provider);

            foreach (var bot in bots)
            {
                bot.Me = me;
                var task = Task.Run(() => action(bot), CancellationToken.None);
                await task;
                if (task.IsFaulted)
                    _logger.LogError(task.Exception, "Bot({BotType}) action({ActionName}) exited unsuccessfully",
                        bot.GetType().Name, action.Method.Name);
            }
        }
    }
}
