using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Seacher.Commons;
using System.Linq;

namespace Seacher;

public partial class AddDBWindow : Window
{
    public AddDBWindow()
    {
        InitializeComponent();
    }

    private void FillFromDBButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dbSettings = new DBSettings(this);
        dbSettings.FillFromDB();

        DataGrid.ItemsSource = dbSettings.DBTables
            .SelectMany(t => t.Columns.Select(c => new { 
                TableName = t.Name, 
                Column = c.Name, 
                DataType = c.DataType })).ToArray();

        dbSettings.Dispose();
    }

    private async void TestButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var dbSettings = new DBSettings(this);
        var resultTestConnection = dbSettings.TestConnection();
        dbSettings.Dispose();

        var box = MessageBoxManager
          .GetMessageBoxStandard("Test connection", resultTestConnection, ButtonEnum.Ok);

        var result = await box.ShowAsync();
    }

    private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}