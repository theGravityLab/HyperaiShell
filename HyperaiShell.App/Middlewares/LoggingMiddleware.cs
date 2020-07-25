using Hyperai.Events;
using Hyperai.Messages;
using Hyperai.Middlewares;
using Hyperai.Services;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Middlewares
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IMessageChainFormatter _formatter;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger, IMessageChainFormatter formatter)
        {
            _logger = logger;
            _formatter = formatter;
        }

        public bool Run(IApiClient client, GenericEventArgs eventArgs)
        {
            switch (eventArgs)
            {
                case GroupMessageEventArgs args:
                    _logger.LogInformation("GroupMessageEventArgs received {0}:\n{1}=>{2}", args.User.DisplayName, args.Group.Name, _formatter.Format(args.Message));
                    break;

                case FriendMessageEventArgs args:
                    _logger.LogInformation("FriendMessageEventArgs received {0}:\n{1}", args.User.Nickname, args.Message.ToString());
                    break;

                default:
                    _logger.LogInformation("GenericEventArgs received at {0}", eventArgs.Time);
                    break;
            }

            return true;
        }
    }
}