using Hyperai.Events;
using Hyperai.Middlewares;
using Hyperai.Services;
using HyperaiShell.Foundation.Services;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Middlewares
{
    public class BlockMiddleware : IMiddleware
    {
        private readonly IBlockService _service;
        private readonly ILogger _logger;

        public BlockMiddleware(IBlockService service, ILogger<BlockMiddleware> logger)
        {
            _service = service;
            _logger = logger;
        }

        public bool Run(IApiClient sender, GenericEventArgs args)
        {
            switch (args)
            {
                case FriendMessageEventArgs friendMessage:
                    {
                        string reason = null;
                        bool banned = _service.IsBanned(friendMessage.User.Identity, out reason);
                        if (banned)
                        {
                            _logger.LogInformation("Message rejected ({}) for {}", friendMessage.Message.ToString(), reason);
                        }

                        return !banned;
                    }
                case GroupMessageEventArgs groupMessage:
                    {
                        string reason = null;
                        bool banned = _service.IsBanned(groupMessage.User.Identity, out reason);
                        if (banned)
                        {
                            _logger.LogInformation("Message rejected: ({}) for {}", groupMessage.Message.ToString(), reason);
                        }

                        return !banned;
                    }
                default:
                    return true;
            }
        }
    }
}