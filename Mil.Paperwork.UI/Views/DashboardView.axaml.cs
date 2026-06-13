using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Mil.Paperwork.UI.ViewModels.History;
using Mil.Paperwork.UI.ViewModels.Tabs;

namespace Mil.Paperwork.UI.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        RecentGrid.AddHandler(KeyDownEvent, OnGridKeyDown, RoutingStrategies.Tunnel);
    }

    private void OnGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        var vm = (DashboardViewModel?)DataContext;
        var selected = RecentGrid.SelectedItem as HistoryEntryViewModel;
        if (vm == null || selected == null)
        {
            return;
        }

        vm.OpenEntryCommand.Execute(selected);
    }

    private void OnGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        var vm = (DashboardViewModel?)DataContext;
        var selected = RecentGrid.SelectedItem as HistoryEntryViewModel;
        if (vm == null || selected == null)
        {
            return;
        }

        vm.OpenEntryCommand.Execute(selected);
        e.Handled = true;
    }

    private void OnFileNameDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not TextBlock tb || tb.DataContext is not HistoryEntryViewModel entry)
        {
            return;
        }

        var vm = (DashboardViewModel?)DataContext;
        if (vm == null)
        {
            return;
        }

        vm.OpenFileFolderCommand.Execute(entry);
        e.Handled = true;
    }
}
