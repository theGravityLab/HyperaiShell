using Hyperai.Events;
using Hyperai.Relations;

namespace HyperaiShell.Foundation.Bots
{
    public abstract class BotBase
    {
        public virtual Self Me { get; set; }

        public virtual void OnFriendMessage(object sender, FriendMessageEventArgs args)
        {
        }

        public virtual void OnGroupMessage(object sender, GroupMessageEventArgs args)
        {
        }

        public virtual void OnGroupRecall(object sender, GroupRecallEventArgs args)
        {
        }

        public virtual void OnFriendRecall(object sender, FriendRecallEventArgs args)
        {
        }

        public virtual void OnFriendRequest(object sender, FriendRequestEventArgs args)
        {
        }

        public virtual void OnMemberRequest(object sender, GroupMemberRequestEventArgs args)
        {
        }

        public virtual void OnEverything(object sender, GenericEventArgs args)
        {
        }
    }
}