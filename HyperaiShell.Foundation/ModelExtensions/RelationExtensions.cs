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

        public static Member GetMemebr(this Group group, long identity)
        {
            return client.RequestAsync(new Member() { Identity = identity, Group = new Lazy<Group>(group) }).GetAwaiter().GetResult();
        }

        public static void Await(this Group group, ActionDelegate action, int msToExpire = 3000)
        {
            unit.WaitOne(Channel.CreateMatchingGroup(group.Identity), action, TimeSpan.FromMilliseconds(msToExpire));
        }

        public static void Await(this Member member, ActionDelegate action, int msToExpire = 3000)
        {
            unit.WaitOne(Channel.Create(member.Identity, member.Group.Value.Identity), action, TimeSpan.FromMilliseconds(msToExpire));
        }
    }
}