using Hyperai.Relations;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class BlacklsitExtensions
    {
        private static readonly IBlockService service;

        static BlacklsitExtensions()
        {
            service = Shared.Application.Provider.GetRequiredService<IBlockService>();
        }

        public static bool IsBanned(this User user)
        {
            return service.IsBanned(user.Identity, out _);
        }

        public static void Ban(this User user, string reason)
        {
            service.Ban(user.Identity, reason);
        }

        public static void Deban(this User user)
        {
            service.Deban(user.Identity);
        }
    }
}