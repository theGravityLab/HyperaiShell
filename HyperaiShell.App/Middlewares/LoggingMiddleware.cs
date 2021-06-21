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
                    _logger.LogInformation("{0} received {1}-{2}:\n{3}", args.GetType().Name, args.Group.Name,
                        args.User.DisplayName, args.Message);
                    break;

                case FriendMessageEventArgs args:
                    _logger.LogInformation("{0} received {1}:\n{2}", args.GetType().Name, args.User.Nickname,
                        args.Message);
                    break;

                case GroupMemberMutedEventArgs args:
                    _logger.LogInformation("{0} received {1}:\n{2} by {3} for {4}", args.GetType().Name,
                        args.Group.Name, args.Whom.DisplayName, args.Operator.DisplayName, args.Duration);
                    break;

                case GroupMemberJoinedEventArgs args:
                    _logger.LogInformation("{0} received {1}:\n{2} by {3}", args.GetType().Name, args.Group.Name,
                        args.Who.DisplayName, args.Operator.DisplayName);
                    break;

                case GroupMemberUnmutedEventArgs args:
                    _logger.LogInformation("{0} received {1}:\n{2} by {3}", args.GetType().Name, args.Group.Name,
                        args.Whom.DisplayName, args.Operator.DisplayName);
                    break;

                default:
                    _logger.LogInformation("{0} received at {1}", eventArgs.GetType().Name, eventArgs.Time);
                    break;
            }

            return true;
        }
    }
}
