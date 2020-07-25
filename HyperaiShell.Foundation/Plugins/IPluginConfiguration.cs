using Microsoft.Extensions.Configuration;

namespace HyperaiShell.Foundation.Plugins
{
    public interface IPluginConfiguration<TPlugin> : IPluginBelonging<TPlugin, IConfiguration> where TPlugin : PluginBase
    {
    }
}