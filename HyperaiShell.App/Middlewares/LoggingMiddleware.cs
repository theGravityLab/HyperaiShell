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

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public bool Run(IApiClient client, GenericEventArgs eventArgs)
        {
            switch (eventArgs)
            {
                case GroupMessageEventArgs args:
                    _logger.LogInformation("GroupMessageEventArgs received {0}-{1}:\n{2}", args.Group.Name,
                        args.User.DisplayName, args.Message.AsReadable().ToString());
                    break;

                case FriendMessageEventArgs args:
                    _logger.LogInformation("FriendMessageEventArgs received {0}:\n{1}", args.User.Nickname,
                        args.Message.AsReadable().ToString());
                    break;
                
                case GroupMemberMutedEventArgs args:
                    _logger.LogInformation($"GroupMemberMutedEventArgs received {0}:\n{1} by {2} for {3}", args.Group.Name, args.Whom.DisplayName, args.Operator.DisplayName, args.Duration);
                    break;
                
                case GroupMemberJoinedEventArgs args:
                    _logger.LogInformation($"GroupMemberJoinedEventArgs received {0}:\n{1} by {2}", args.Group.Name, args.Who.DisplayName, args.Operator.DisplayName);
                    break;
                
                case GroupMemberUnmutedEventArgs args:
                    _logger.LogInformation($"GroupMemberUnmutedEventArgs received {0}:\n{1} by {2}", args.Group.Name, args.Whom.DisplayName, args.Operator.DisplayName);
                    break;

                default:
                    _logger.LogInformation("{0} received at {1}", eventArgs.GetType().Name, eventArgs.Time);
                    break;
            }

            return true;
        }
    }
}
