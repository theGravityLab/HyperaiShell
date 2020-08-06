using Hyperai.Relations;
using Hyperai.Services;
using Hyperai.Units;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class RelationExtensions
    {
        private static readonly IApiClient client;
        private static readonly IUnitService unit;

        static RelationExtensions()
        {
            client = Shared.Application.Provider.GetRequiredService<IApiClient>();
            unit = Shared.Application.Provider.GetRequiredService<IUnitService>();
        }

        /// <summary>
        /// 在确定群内有该成员的前提下用其 id 获取成员其他信息
        /// </summary>
        /// <param name="group">目标群</param>
        /// <param name="identity">TA滴 id</param>
        /// <returns>完整的群员信息</returns>
        public static Member GetMemebr(this Group group, long identity)
        {
            return client.RequestAsync(new Member() { Identity = identity, Group = new Lazy<Group>(group) }).GetAwaiter().GetResult();
        }

        /// <summary>
        /// 监听该 <see cref="Group"> 的下一条消息
        /// </summary>
        /// <param name="group">目标群</param>
        /// <param name="action">当消息抵达时的操作</param>
        /// <param name="msToExpire">过期时间(ms)</param>
        public static void Await(this Group group, ActionDelegate action, int msToExpire = 3000)
        {
            unit.WaitOne(Channel.CreateMatchingGroup(group.Identity), action, TimeSpan.FromMilliseconds(msToExpire));
        }

        /// <summary>
        /// 监听该 <see cref="Member"> 的下一条消息
        /// </summary>
        /// <param name="member">目标成员</param>
        /// <param name="action">当消息抵达时的操作</param>
        /// <param name="msToExpire">过期时间(ms)</param>
        public static void Await(this Member member, ActionDelegate action, int msToExpire = 3000)
        {
            unit.WaitOne(Channel.Create(member.Identity, member.Group.Value.Identity), action, TimeSpan.FromMilliseconds(msToExpire));
        }
    }
}