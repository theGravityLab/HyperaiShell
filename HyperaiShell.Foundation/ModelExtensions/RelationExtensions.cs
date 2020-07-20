using Hyperai.Relations;
using Hyperai.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class RelationExtensions
    {
        private static readonly IApiClient client;
        static RelationExtensions()
        {
            client = Shared.Application.Provider.GetRequiredService<IApiClient>();
        }
        public static Member GetMemebr(this Group group, long identity)
        {
            return client.RequestAsync(new Member() { Identity = identity, Group = group }).GetAwaiter().GetResult();
        }
    }
}
