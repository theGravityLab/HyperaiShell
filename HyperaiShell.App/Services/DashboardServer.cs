using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HyperaiShell.App.DashboardInterface;
using HyperaiShell.App.DashboardInterface.Scenarios;
using HyperaiShell.Foundation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Terminal.Gui;
using static Terminal.Gui.TabView;

namespace HyperaiShell.App.Services
{
    public class DashboardServer: IHostedService
    {
        private readonly ILogger _logger;

        private readonly IEnumerable<ScenarioBase> _scenarios;

        public DashboardServer(ILogger<DashboardServer> logger,IEnumerable<ScenarioBase> scenarios)
        {
            _logger = logger;
            _scenarios = scenarios;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Application.Init();
            // var listView = new ListView()
            // {
            //     Height = 1,
            //     Width = Dim.Fill(),
                
            // };
            // listView.Source = new ListWrapper(_scenarios.Select(x => {x.OnCreated(); return x;}).ToList())
            // {

            // };
            // Application.Top.Add(listView);

            var scenarios = _scenarios.ToArray();

            var tabControl = new TabView()
            {
                Height = Dim.Fill() - 1,
                Width = Dim.Fill(),
                CanFocus = true
            };

            var statusBar = new StatusBar()
            {
                Width = Dim.Fill(),
                Height = 1
            };

            

            for(int i = 0; i < scenarios.Length; i ++)
            {
                scenarios[i].OnCreated();
                tabControl.AddTab(new Tab(scenarios[i].Header.ToString(), scenarios[i]), i == 0);
            }

            tabControl.SelectedTabChanged += (sender, args) =>
            {
                statusBar.Items = ((ScenarioBase)args.NewTab.View).StatusBarItems.ToArray(); 
            };

            statusBar.Items = ((ScenarioBase)tabControl.SelectedTab.View).StatusBarItems.ToArray(); 

            Application.Top.Add(tabControl);
            Application.Top.Add(statusBar);
            Application.Run();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopped");
            return Task.CompletedTask;
        }
    }
}
