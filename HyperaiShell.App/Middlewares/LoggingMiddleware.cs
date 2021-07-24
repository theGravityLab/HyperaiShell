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
                    _logger.LogInformation("{} received {}-{}:\n{}", args, args.Group,
                        args.User, args.Message);
                    break;

                case FriendMessageEventArgs args:
                    _logger.LogInformation("{} received {}:\n{}", args, args.User,
                        args.Message);
                    break;

                case GroupMemberMutedEventArgs args:
                    _logger.LogInformation("{} received {}:\n{} by {} for {}", args,
                        args.Group, args.Whom, args.Operator, args.Duration);
                    break;

                case GroupMemberJoinedEventArgs args:
                    _logger.LogInformation("{} received {}:\n{} by {}", args, args.Group,
                        args.Who, args.Operator);
                    break;

                case GroupMemberUnmutedEventArgs args:
                    _logger.LogInformation("{} received {}:\n{} by {}", args, args.Group,
                        args.Whom, args.Operator);
                    break;

                default:
                    _logger.LogInformation("{} received at {}", eventArgs, eventArgs.Time);
                    break;
            }

            return true;
        }
    }
}
