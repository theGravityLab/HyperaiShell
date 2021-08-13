using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

namespace HyperaiShell.App.Services
{
    public class DashboardServer: IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var table = new Table();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
