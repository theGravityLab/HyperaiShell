using HyperaiShell.Foundation.Plugins;
using Microsoft.Extensions.Configuration;
using System;

namespace HyperaiShell.App.Plugins
{
    public class PluginConfiguration<TPlugin> : IPluginConfiguration<TPlugin> where TPlugin : PluginBase
    {
        public Type BelongingTo =>
            typeof(TPlugin);

        public IConfiguration Value =>
            PluginManager.Instance.GetContextOfPlugin(typeof(TPlugin)).Configuration.Value;
    }
}