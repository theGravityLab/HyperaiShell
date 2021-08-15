using Hangfire.Logging;
using HyperaiShell.App.Logging;
using Terminal.Gui;

namespace HyperaiShell.App.DashboardInterface.Scenarios
{
    public class LogScenario : ScenarioBase
    {
        public override void OnCreated()
        {
            base.OnCreated();

            Header = "Log";




            var label = new Label()
            {
                Height = Dim.Fill(),
                Width = Dim.Percent(50f),
            };

            var list = new ListView()
            {
                Height = Dim.Fill(),
                Width = Dim.Percent(50f),
                X = Pos.Right(label)
            };

            list.Source = new ListWrapper(DashboardLoggingStore.Instance.Logs);

            list.SelectedItemChanged += (args) => 
            {
                var value = (LogItem)args.Value;
                label.Text = $"Level: {value.Level}\n"
                + $"Source: {value.Source}\n"
                + $"DateTime: {value.Time}\n"
                + $"Message: {value.Message}";

                if(value.Exception != null)
                {
                    label.Text += $"\nException: {value.Exception}";
                }
            };

            Add(list);
            Add(label);
        }
    }
}
