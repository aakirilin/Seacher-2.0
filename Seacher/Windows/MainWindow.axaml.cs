using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Seacher.Commons;
using System.Diagnostics;

namespace Seacher
{
    public partial class MainWindow : Window
    {
        private readonly Settings settings;

        public MainWindow(Settings settings)
        {
            InitializeComponent();
            this.settings = settings;

            SelectTable.SelectionChanged += (s, e) => {
                var index = SelectTable.SelectedIndex;

            };


            UpdateSelectTableComboBox();

        }

        private void Settings_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var window = new SettingsWindow(settings);
            window.Show();
            window.Closed += (s, e) =>
            {
                UpdateSelectTableComboBox();
            };
        }

        private void UpdateSelectTableComboBox()
        {
            SelectTable.Items.Clear();
            foreach (var db in settings.DBSettings)
            {
                foreach (var table in db.DBTables)
                {
                    var title = $"{db.Name}->{table.Name}";
                    SelectTable.Items.Add(new ComboBoxItem() { Content = title });
                }
            }

            Conditions.Children.Clear();


        }
    }
}