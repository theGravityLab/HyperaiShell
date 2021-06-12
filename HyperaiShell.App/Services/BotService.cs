using System;
using System.Threading;
using System.Threading.Tasks;
using Hyperai.Events;
using Hyperai.Relations;
using Hyperai.Services;
using HyperaiShell.App.Bots;
using HyperaiShell.Foundation.Bots;
using HyperaiShell.Foundation.Services;

namespace HyperaiShell.App.Services
{
    public class BotService : IBotService
    {
        private readonly IApiClient _client;
        private readonly IServiceProvider _provider;
        private BotCollection bots;

        public BotService(IApiClient client, IServiceProvider provider)
        {
            _client = client;
            _provider = provider;
        }

        public IBotCollectionBuilder Builder { get; } = new BotCollectionBuilder();

        public async Task PushAsync(GenericEventArgs args)
        {
            var self = await _client.RequestAsync<Self>(null);
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

                case GroupMemberRequestEventArgs groupMemberRequest:
                    DoForAll(x => x.OnMemberRequest(_client, groupMemberRequest), self);
                    break;

                case FriendRequestResponsedEventArgs friendRequestResp:
                    DoForAll(x => x.OnFriendRequestResp(_client, friendRequestResp), self);
                    break;

                case MemberRequestResponsedEventArgs memberRequestResp:
                    DoForAll(x => x.OnMemberRequestResp(_client, memberRequestResp), self);
                    break;

                case GroupJoinedEventArgs groupJoined:
                    DoForAll(x => x.OnGroupJoined(_client, groupJoined), self);
                    break;

                case GroupMemberJoinedEventArgs groupMemberJoined:
                    DoForAll(x => x.OnGroupMemberJoined(_client, groupMemberJoined), self);
                    break;

                case GroupMemberLeftEventArgs groupMemberLeft:
                    DoForAll(x => x.OnGroupMemberLeft(_client, groupMemberLeft), self);
                    break;

                case GroupLeftEventArgs groupLeft:
                    DoForAll(x => x.OnGroupLeft(_client, groupLeft), self);
                    break;

                case GroupMemberCardChangedEventArgs groupMemberCardChanged:
                    DoForAll(x => x.OnGroupMemberCardChanged(_client, groupMemberCardChanged), self);
                    break;

                case GroupMemberTitleChangedEventArgs groupMemberTitleChanged:
                    DoForAll(x => x.OnGroupMemberTitleChanged(_client, groupMemberTitleChanged), self);
                    break;

                case GroupMutedEventArgs groupMuted:
                    DoForAll(x => x.OnGroupMuted(_client, groupMuted), self);
                    break;

                case GroupNameChangedEventArgs groupNameChanged:
                    DoForAll(x => x.OnGroupNameChanged(_client, groupNameChanged), self);
                    break;

                case GroupPermissionChangedEventArgs groupPermissionChanged:
                    DoForAll(x => x.OnGroupPermissionChanged(_client, groupPermissionChanged), self);
                    break;

                case GroupUnmutedEventArgs groupUnmuted:
                    DoForAll(x => x.OnGroupUnmuted(_client, groupUnmuted), self);
                    break;

                case InvitationResponsedEventArgs invitationResp:
                    DoForAll(x => x.OnInvitationResp(_client, invitationResp), self);
                    break;
            }
            DoForAll( x=> x.OnEverything(_client, args), self);
        }

        private void DoForAll(Action<BotBase> action, Self me)
        {
            bots ??= Builder.Build(_provider);

            foreach (var bot in bots)
            {
                bot.Me = me;
                Task.Run(() => action(bot), CancellationToken.None);
            }
        }
    }
}
