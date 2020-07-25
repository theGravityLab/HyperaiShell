using Hyperai.Messages;
using Microsoft.Extensions.DependencyInjection;

namespace HyperaiShell.Foundation.ModelExtensions
{
    public static class MessageChainExtensions
    {
        public static string Flatten(this MessageChain message)
        {
            return Shared.Application.Provider.GetRequiredService<IMessageChainFormatter>().Format(message);
        }

        public static MessageChain MakeMessageChain(this string code)
        {
            return Shared.Application.Provider.GetRequiredService<IMessageChainParser>().Parse(code);
        }
    }
}