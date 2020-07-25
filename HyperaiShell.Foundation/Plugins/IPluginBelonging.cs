using System;

namespace HyperaiShell.Foundation.Plugins
{
    public interface IPluginBelonging<TPlugin, TBelonging> where TPlugin : PluginBase
    {
        Type Plugin => typeof(TPlugin);
        TBelonging Value { get; }
    }
}