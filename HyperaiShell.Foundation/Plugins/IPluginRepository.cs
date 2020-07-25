using HyperaiShell.Foundation.Data;

namespace HyperaiShell.Foundation.Plugins
{
    public interface IPluginRepository<TPlugin> : IPluginBelonging<TPlugin, IRepository> where TPlugin : PluginBase
    {
    }
}