using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperai.Services;
using Hyperai.Units;
using HyperaiShell.App.Data;
using HyperaiShell.App.Packages;
using HyperaiShell.App.Plugins;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.Plugins;
using HyperaiShell.Foundation.Services;
using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;

namespace HyperaiShell.App
{
    public class Program
    {

        public async static Task Main()
        {
            // search packages and load
            foreach (var nupkg in Directory.GetFiles("plugins/", "*.nupkg"))
            {
                await PackageManager.Instance.LoadPackageAsync(nupkg);
            }
        }
    }
}
