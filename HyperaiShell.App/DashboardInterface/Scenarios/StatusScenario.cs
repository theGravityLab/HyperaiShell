using System.Text;
using Hyperai.Services;
using HyperaiShell.Foundation;
using HyperaiShell.Foundation.Plugins;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;

namespace HyperaiShell.App.DashboardInterface.Scenarios
{
    public class StatusScenario : ScenarioBase
    {
        private readonly IApiClient _client;
        private readonly IHostEnvironment _env;
        public StatusScenario(IApiClient client, IHostEnvironment env)
        {
            _client = client;
            _env = env;
        }
        public override void OnCreated()
        {
            base.OnCreated();

            Header = "Status";

            StatusBarItems.Add(new StatusItem(Key.CtrlMask | Key.C, "^C Quit", () =>
            {
                Application.RequestStop();
                Shared.Host.StopAsync().Wait();
            }));

            var scrollView = new ScrollView()
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                ShowVerticalScrollIndicator = true,
            };

            var sb = new StringBuilder();
            sb.AppendLine("[Application]");
            sb.AppendLine("Enviroment=" + _env.EnvironmentName);
            sb.AppendLine("ContentRootPath=" + _env.ContentRootPath);
            sb.AppendLine("[Version]");
            sb.AppendLine("Hyperai/" + typeof(IApiClient).Assembly.GetName().Version);
            sb.AppendLine("HyperaiShell/" + typeof(PluginBase).Assembly.GetName().Version);
            sb.AppendLine(_client.GetType().Name + "/" + typeof(IApiClient).Assembly.GetName().Version);

            var mainText = new Label()
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
                Text = sb.ToString()
            };

            scrollView.Text = sb.ToString();
            Add(scrollView);
        }
    }
}
