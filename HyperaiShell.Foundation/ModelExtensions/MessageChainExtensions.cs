using Hyperai.Messages;
using Hyperai.Messages.ConcreteModels;
using Hyperai.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class MessageChainExtensions
    {
        private readonly static IMessageChainFormatter _formatter;
        private readonly static IMessageChainParser _parser;
        private readonly static IApiClient _client;

        static MessageChainExtensions()
        {
            _formatter = Shared.Application.Provider.GetRequiredService<IMessageChainFormatter>();
            _parser = Shared.Application.Provider.GetRequiredService<IMessageChainParser>();
            _client = Shared.Application.Provider.GetRequiredService<IApiClient>();
        }

        /// <summary>
        /// 使用默认 <see cref="IMessageChainFormatter"/> 格式化消息链
        /// </summary>
        /// <param name="message">消息链</param>
        /// <returns>消息文本</returns>
        public static string Flatten(this MessageChain message)
        {
            return _formatter.Format(message);
        }

        /// <summary>
        /// 使用默认 <see cref="IMessageChainParser"/> 解析消息文本
        /// </summary>
        /// <param name="code">消息文本</param>
        /// <returns>消息链</returns>
        public static MessageChain MakeMessageChain(this string code)
        {
            return _parser.Parse(code);
        }

        /// <summary>
        /// 撤回该消息, 如果该消息不含 <see cref="Source"/> 则引发异常
        /// </summary>
        /// <param name="chain">包含 <see cref="Source"/> 的消息链</param>
        /// <returns><see cref="Task"/></returns>
        public static async Task RevokeAsync(this MessageChain chain)
        {
            await _client.RevokeMessageAsync(((Source)chain.First(x => x is Source)).MessageId);
        }
    }
}