using HyperaiShell.Foundation.Bots;
using Microsoft.Extensions.DependencyInjection;

namespace HyperaiShell.Foundation.Plugins
{
    public abstract class PluginBase
    {
        public virtual IPluginContext Context { get; set; }

        public abstract void ConfigureBots(IBotCollectionBuilder bots);

        public abstract void ConfigureServices(IServiceCollection services);
    }
}