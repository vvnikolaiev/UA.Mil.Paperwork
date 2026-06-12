using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Mil.Paperwork.UI.ViewModels.History;
using Mil.Paperwork.UI.ViewModels.Tabs;
using System.ComponentModel;

namespace Mil.Paperwork.UI.Views;

public partial class HistoryView : UserControl
{
    public HistoryView()
    {
        InitializeComponent();
        HistoryGrid.AddHandler(KeyDownEvent, OnGridKeyDown, RoutingStrategies.Tunnel);
    }

    private void OnGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        var vm = (HistoryViewModel?)DataContext;
        if (vm?.SelectedEntry == null)
        {
            return;
        }

        vm.OpenEntryCommand.Execute(vm.SelectedEntry);
    }

    private void OnGridKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        var vm = (HistoryViewModel?)DataContext;
        if (vm?.SelectedEntry == null)
        {
            return;
        }

        vm.OpenEntryCommand.Execute(vm.SelectedEntry);
        e.Handled = true;
    }

    private void OnFileNameDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not TextBlock tb || tb.DataContext is not HistoryEntryViewModel entry)
        {
            return;
        }

        var vm = (HistoryViewModel?)DataContext;
        if (vm == null)
        {
            return;
        }

        vm.OpenFileFolderCommand.Execute(entry);
        e.Handled = true;
    }

    private void OnGridSorting(object? sender, DataGridColumnEventArgs e)
    {
        e.Handled = true;

        var memberPath = e.Column.SortMemberPath;
        if (string.IsNullOrEmpty(memberPath))
        {
            return;
        }

        var vm = (HistoryViewModel?)DataContext;
        if (vm == null)
        {
            return;
        }

        var newDirection = vm.CurrentSortMemberPath == memberPath && vm.CurrentSortDirection == ListSortDirection.Ascending
            ? ListSortDirection.Descending
            : ListSortDirection.Ascending;

        vm.ApplySortMember(memberPath, newDirection);
    }
}
