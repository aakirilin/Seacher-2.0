using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Seacher.Commons;
using System.Linq;

namespace Seacher;

public partial class SettingsWindow : Window
{
    private readonly Settings settings;

    public SettingsWindow(Settings settings)
    {
        InitializeComponent();
        this.settings = settings;

        ApdateDBDataGrid();
        DBDataGrid.SelectionChanged += (object? sender, SelectionChangedEventArgs e) =>
        {
            if (DBDataGrid.SelectedIndex > -1)
            {
                TablesDataGrid.ItemsSource = settings.DBSettings[DBDataGrid.SelectedIndex]
                    .DBTables.Select(t => new { TableName = t.Name });                
            }
        };
        TablesDataGrid.SelectionChanged += (object? sender, SelectionChangedEventArgs e) =>
        {
            if (DBDataGrid.SelectedIndex > -1 && TablesDataGrid.SelectedIndex > -1)
            {
                ColumnsDataGrid.ItemsSource = settings.DBSettings[DBDataGrid.SelectedIndex]
                    .DBTables[TablesDataGrid.SelectedIndex].Columns;
            }
        };
    }

    private async void AddDBButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = new AddDBWindow();
        await window.ShowDialog(this);

        var db = new DBSettings(window);
        db.FillFromDB();
        settings.DBSettings.Add(db);
        ApdateDBDataGrid();
    }

    private void ApdateDBDataGrid()
    {
        DBDataGrid.ItemsSource = settings.DBSettings.Select(db => new { DBName = db.Name }).ToArray();
    }

    private void SetInQerryButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ColumnsDataGrid.SelectedIndex > -1)
        {
            var column = settings.DBSettings[DBDataGrid.SelectedIndex]
                .DBTables[TablesDataGrid.SelectedIndex].Columns[ColumnsDataGrid.SelectedIndex];

            column.ShowInData = !column.ShowInData;
            // костыль !!!
            var selIndex = TablesDataGrid.SelectedIndex;
            TablesDataGrid.SelectedIndex = selIndex == 0 ? 1 : 0;
            TablesDataGrid.SelectedIndex = selIndex;
        }
    }

    private void SetInCondition_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (ColumnsDataGrid.SelectedIndex > -1)
        {
            if (ColumnsDataGrid.SelectedIndex > -1)
            {
                var column = settings.DBSettings[DBDataGrid.SelectedIndex]
                    .DBTables[TablesDataGrid.SelectedIndex].Columns[ColumnsDataGrid.SelectedIndex];

                column.ShowInCondition = !column.ShowInCondition;
                // костыль !!!
                var selIndex = TablesDataGrid.SelectedIndex;
                TablesDataGrid.SelectedIndex = selIndex == 0 ? 1 : 0;
                TablesDataGrid.SelectedIndex = selIndex;
            }
        }
    }

    private void DeleteDBButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DBDataGrid.SelectedIndex > -1)
        {
            settings.DBSettings.RemoveRange(DBDataGrid.SelectedIndex, 1);
            ApdateDBDataGrid();
        }
    }
}