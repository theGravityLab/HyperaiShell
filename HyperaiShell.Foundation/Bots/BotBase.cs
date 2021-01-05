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

        public virtual void OnFriendRequestResp(object sender, FriendRequestResponsedEventArgs args)
        {
        }
        
        public virtual void OnMemberRequest(object sender, GroupMemberRequestEventArgs args)
        {
        }
        
        public virtual void OnMemberRequestResp(object sender, MemberRequestResponsedEventArgs args)
        {
        }
        
        public virtual void OnGroupJoined(object sender, GroupJoinedEventArgs args)
        {
        }
        
        public virtual void OnGroupLeft(object sender, GroupLeftEventArgs args)
        {
        }
        
        public virtual void OnGroupMemberCardChanged(object sender, GroupMemberCardChangedEventArgs args)
        {
        }
        
        public virtual void OnGroupMemberTitleChanged(object sender, GroupMemberTitleChangedEventArgs args)
        {
        }

        public virtual void OnGroupMuted(object sender, GroupMutedEventArgs args)
        {
        }
        
        public virtual void OnGroupNameChanged(object sender, GroupNameChangedEventArgs args)
        {
        }
        
        public virtual void OnGroupPermissionChanged(object sender, GroupPermissionChangedEventArgs args)
        {
        }
        
        public virtual void OnGroupUnmuted(object sender, GroupUnmutedEventArgs args)
        {
        }
        
        public virtual void OnInvitationResp(object sender, InvitationResponsedEventArgs args)
        {
        }

        public virtual void OnEverything(object sender, GenericEventArgs args)
        {
        }
    }
}