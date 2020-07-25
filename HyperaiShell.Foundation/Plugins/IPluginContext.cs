using HyperaiShell.Foundation.Data;
using Microsoft.Extensions.Configuration;
using System;

namespace HyperaiShell.Foundation.Plugins
{
    public interface IPluginContext
    {
        PluginMeta Meta { get; }
        Lazy<IConfiguration> Configuration { get; }
        Lazy<IRepository> Repository { get; }
    }
}