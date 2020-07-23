using Hyperai.Events;
using Hyperai.Middlewares;
using Hyperai.Services;
using HyperaiShell.Foundation.Services;

namespace HyperaiShell.App.Middlewares
{
    public class BlockMiddleware : IMiddleware
    {
        private readonly IBlockService _service;

        public BlockMiddleware(IBlockService service)
        {
            _service = service;
        }

        public bool Run(IApiClient sender, GenericEventArgs args)
        {
            return args switch
            {
                FriendMessageEventArgs friendMessage => !_service.IsBanned(friendMessage.User.Identity, out _),
                GroupMessageEventArgs groupMessage => !_service.IsBanned(groupMessage.User.Identity, out _),
                _ => true
            };
        }
    }
}