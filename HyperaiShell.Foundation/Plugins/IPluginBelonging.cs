using System;

namespace HyperaiShell.Foundation.Plugins
{
    public interface IPluginBelonging<TPlugin, out TBelonging> where TPlugin : PluginBase
    {
        Type Plugin => typeof(TPlugin);
        TBelonging Value { get; }
    }
}
