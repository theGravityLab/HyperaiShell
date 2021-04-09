using Hyperai.Events;
using Hyperai.Middlewares;
using Hyperai.Services;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.App.Middlewares
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public bool Run(IApiClient client, GenericEventArgs eventArgs)
        {
            switch (eventArgs)
            {
                case GroupMessageEventArgs args:
                    _logger.LogInformation("GroupMessageEventArgs received {0}:\n{1}=>{2}", args.User.DisplayName,
                        args.Group.Name, args.Message);
                    break;

                case FriendMessageEventArgs args:
                    _logger.LogInformation("FriendMessageEventArgs received {0}:\n{1}", args.User.Nickname,
                        args.Message);
                    break;

                default:
                    _logger.LogInformation("{0} received at {1}", eventArgs.GetType().Name, eventArgs.Time);
                    break;
            }

            return true;
        }
    }
}