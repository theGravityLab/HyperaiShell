using System.Collections;
using System.Collections.Generic;
using Terminal.Gui;

namespace HyperaiShell.App.DashboardInterface.Scenarios
{
    public abstract class ScenarioBase : View
    {
        public string Header { get; set; }
        public List<StatusItem> StatusBarItems { get; set;} = new List<StatusItem>();
        public virtual void OnCreated()
        {
            Height = Dim.Fill();
            Width = Dim.Fill();
        }
    }
}
