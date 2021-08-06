using Hyperai.Events;
using Hyperai.Relations;
using Hyperai.Services;

namespace HyperaiShell.Foundation.Bots
{
    public abstract class BotBase
    {
        public virtual Self Me { get; set; }

        public virtual void OnEverything(object sender, GenericEventArgs args)
        {
        }

        public virtual void OnFriendMessage(IApiClient client, FriendMessageEventArgs args)
        {
        }

        public virtual void OnGroupMessage(IApiClient client, GroupMessageEventArgs args)
        {
        }

        public virtual void OnFriendRecall(IApiClient client, FriendRecallEventArgs args)
        {
        }

        public virtual void OnGroupRecall(IApiClient client, GroupRecallEventArgs args)
        {
        }

        public virtual void OnGroupLeft(IApiClient client, GroupLeftEventArgs args)
        {
        }

        public virtual void OnGroupJoined(IApiClient client, GroupJoinedEventArgs args)
        {
        }

        public virtual void OnGroupMemberMuted(IApiClient client, GroupMemberMutedEventArgs args)
        {
        }

        public virtual void OnGroupMemberUnmuted(IApiClient client, GroupMemberUnmutedEventArgs args)
        {
        }

        public virtual void OnGroupAllMuted(IApiClient client, GroupAllMutedEventArgs args)
        {
        }

        public virtual void OnGroupMemberCardChanged(IApiClient client, GroupMemberCardChangedEventArgs args)
        {
        }

        public virtual void OnGroupMemberTitleChanged(IApiClient client, GroupMemberTitleChangedEventArgs args)
        {
        }

        public virtual void OnGroupPermissionChanged(IApiClient client, GroupPermissionChangedEventArgs args)
        {
        }
    }
}
