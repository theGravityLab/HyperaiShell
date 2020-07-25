using Hyperai.Events;
using Hyperai.Messages;
using Hyperai.Messages.ConcreteModels;
using Hyperai.Middlewares;
using Hyperai.Services;
using System.Linq;

namespace HyperaiShell.App.Middlewares
{
    public class TranslatorMiddleware : IMiddleware
    {
        private readonly IMessageChainParser _parser;

        public TranslatorMiddleware(IMessageChainParser parser)
        {
            _parser = parser;
        }

        public bool Run(IApiClient sender, GenericEventArgs args)
        {
            if (args is MessageEventArgs msgEvent)
            {
                string text = string.Join(string.Empty, msgEvent.Message.OfType<Plain>().Select(x => x.Text));
                if (text.Length > 8 && (text.StartsWith("```\r") || text.StartsWith("```\n")) && (text.EndsWith("\r```") || text.EndsWith("\n```")))
                {
                    msgEvent.Message = _parser.Parse(text.Substring(4, text.Length - 8));
                }
            }
            return true;
        }
    }
}