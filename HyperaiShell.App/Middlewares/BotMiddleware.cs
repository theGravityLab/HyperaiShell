using Hyperai.Events;
using Hyperai.Middlewares;
using Hyperai.Services;
using HyperaiShell.Foundation.Services;

namespace HyperaiShell.App.Middlewares
{
    public class BotMiddleware : IMiddleware
    {
        private readonly IBotService _service;

        public BotMiddleware(IBotService service)
        {
            _service = service;
        }

        public bool Run(IApiClient sender, GenericEventArgs args)
        {
            _service.PushAsync(args);
            return true;
        }
    }
}