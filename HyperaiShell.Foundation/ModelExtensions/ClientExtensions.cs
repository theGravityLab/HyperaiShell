﻿using System.Threading.Tasks;
using Hyperai.Messages;
using Hyperai.Messages.ConcreteModels;
using Hyperai.Relations;
using Hyperai.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class ClientExtensions
    {
        private static readonly IApiClient _client;
        private static readonly ILogger _logger;

        static ClientExtensions()
        {
            _client = Shared.Application.Provider.GetRequiredService<IApiClient>();
            _logger = Shared.Application.Provider.GetRequiredService<ILoggerFactory>()
                .CreateLogger(typeof(ClientExtensions).AssemblyQualifiedName);
        }

        public static async Task SendAsync(this Friend friend, MessageChain message)
        {
            _logger.LogInformation("{0}({1}) < {2}:\n{3}", _client.GetType().Name, nameof(Friend), friend.Identifier,
                message);
            await _client.SendFriendMessageAsync(friend, message);
        }

        public static async Task SendPlainAsync(this Friend friend, string plain)
        {
            _logger.LogInformation("{0}({1}) < {2}:\n{3}", _client.GetType().Name, nameof(Friend), friend.Identifier,
                plain);
            await _client.SendFriendMessageAsync(friend, new MessageChain(new MessageElement[] {new Plain(plain)}));
        }

        public static async Task SendAsync(this Group group, MessageChain message)
        {
            _logger.LogInformation("{0}({1}) < {2}:\n{3}", _client.GetType().Name, nameof(Group), group.Identifier,
                message);
            await _client.SendGroupMessageAsync(group, message);
        }

        public static async Task SendPlainAsync(this Group group, string plain)
        {
            _logger.LogInformation("{0}(1) < {2}:\n{3}", _client.GetType().Name, nameof(Group), group.Identifier,
                plain);
            await _client.SendGroupMessageAsync(group, new MessageChain(new MessageElement[] {new Plain(plain)}));
        }
    }
}
