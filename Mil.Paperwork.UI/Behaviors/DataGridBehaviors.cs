using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Mil.Paperwork.UI.Behaviors;

public static class DataGridBehaviors
{
    private const string EnableInstantEditName = "EnableInstantEdit";

    private static readonly ConditionalWeakTable<DataGrid, GridEditState> _editStates = new();

    private sealed class GridEditState
    {
        public bool IsEditing;
        public string? PendingText;
    }

    public static readonly AttachedProperty<bool> EnableInstantEditProperty =
        AvaloniaProperty.RegisterAttached<DataGrid, bool>(EnableInstantEditName, typeof(DataGridBehaviors));

    static DataGridBehaviors()
    {
        EnableInstantEditProperty.Changed.AddClassHandler<DataGrid>(OnEnableInstantEditChanged);
        InputElement.GotFocusEvent.AddClassHandler<DataGridCell>(OnCellGotFocus, RoutingStrategies.Bubble);
    }

    public static bool GetEnableInstantEdit(DataGrid grid)
    {
        return grid.GetValue(EnableInstantEditProperty);
    }

    public static void SetEnableInstantEdit(DataGrid grid, bool value)
    {
        grid.SetValue(EnableInstantEditProperty, value);
    }

    private static GridEditState GetState(DataGrid grid)
    {
        return _editStates.GetOrCreateValue(grid);
    }

    private static void OnEnableInstantEditChanged(DataGrid grid, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is true)
        {
            grid.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            grid.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Bubble);
            grid.CellPointerPressed += OnCellPointerPressed;
            grid.PreparingCellForEdit += OnPreparingCellForEdit;
            grid.CellEditEnded += OnCellEditEnded;
        }
        else
        {
            grid.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            grid.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
            grid.CellPointerPressed -= OnCellPointerPressed;
            grid.PreparingCellForEdit -= OnPreparingCellForEdit;
            grid.CellEditEnded -= OnCellEditEnded;
        }
    }

    private static void OnCellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        if (sender is not DataGrid grid || grid.IsReadOnly || e.Column?.IsReadOnly == true)
        {
            return;
        }

        grid.BeginEdit();
    }

    private static void OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (sender is not DataGrid grid)
        {
            return;
        }

        var state = GetState(grid);
        state.IsEditing = true;

        if (state.PendingText != null && e.EditingElement is TextBox tb)
        {
            var text = state.PendingText;
            state.PendingText = null;
            Dispatcher.UIThread.Post(() =>
            {
                tb.Focus();
                tb.Text = text;
                tb.CaretIndex = text.Length;
            });
        }
        else
        {
            Dispatcher.UIThread.Post(() => e.EditingElement?.Focus());
        }
    }

    private static void OnCellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        if (sender is DataGrid grid)
        {
            GetState(grid).IsEditing = false;
        }
    }

    private static void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (sender is not DataGrid grid || grid.IsReadOnly)
        {
            return;
        }

        if (string.IsNullOrEmpty(e.Text))
        {
            return;
        }

        if (grid.CurrentColumn is not DataGridTextColumn || grid.CurrentColumn.IsReadOnly)
        {
            return;
        }

        var state = GetState(grid);
        if (state.IsEditing)
        {
            return;
        }

        e.Handled = true;
        state.PendingText = e.Text;
        grid.BeginEdit();
    }

    private static void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not DataGrid grid || grid.IsReadOnly)
        {
            return;
        }

        if (e.Key == Key.Tab)
        {
            e.Handled = true;
            var backward = (e.KeyModifiers & KeyModifiers.Shift) != 0;
            CommitAndMoveToNextCell(grid, backward);
        }
        else if (e.Key == Key.Enter)
        {
            e.Handled = true;
            HandleEnterKey(grid);
        }
    }

    private static void HandleEnterKey(DataGrid grid)
    {
        var state = GetState(grid);
        var isTemplateColumn = grid.CurrentColumn is DataGridTemplateColumn;

        if (state.IsEditing || isTemplateColumn)
        {
            CommitAndMoveToNextRow(grid);
        }
        else
        {
            grid.BeginEdit();
        }
    }

    private static void CommitAndMoveToNextCell(DataGrid grid, bool backward)
    {
        grid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);

        var columns = grid.Columns;
        if (columns.Count == 0)
        {
            return;
        }

        var items = grid.ItemsSource as IList;
        if (items == null || items.Count == 0)
        {
            return;
        }

        var currentColIndex = grid.CurrentColumn != null ? columns.IndexOf(grid.CurrentColumn) : 0;
        var currentRowIndex = grid.SelectedIndex < 0 ? 0 : grid.SelectedIndex;

        int nextColIndex;
        int nextRowIndex;

        if (!backward)
        {
            nextColIndex = currentColIndex + 1;
            nextRowIndex = currentRowIndex;
            if (nextColIndex >= columns.Count)
            {
                nextColIndex = 0;
                nextRowIndex = currentRowIndex + 1;
                if (nextRowIndex >= items.Count)
                {
                    nextRowIndex = items.Count - 1;
                    nextColIndex = columns.Count - 1;
                }
            }
        }
        else
        {
            nextColIndex = currentColIndex - 1;
            nextRowIndex = currentRowIndex;
            if (nextColIndex < 0)
            {
                nextColIndex = columns.Count - 1;
                nextRowIndex = currentRowIndex - 1;
                if (nextRowIndex < 0)
                {
                    nextRowIndex = 0;
                    nextColIndex = 0;
                }
            }
        }

        var targetColumn = columns[nextColIndex];
        var targetItem = items[nextRowIndex];

        grid.SelectedIndex = nextRowIndex;
        grid.CurrentColumn = targetColumn;
        grid.ScrollIntoView(targetItem!, targetColumn);

        if (!targetColumn.IsReadOnly)
        {
            Dispatcher.UIThread.Post(() => grid.BeginEdit());
        }
    }

    private static void CommitAndMoveToNextRow(DataGrid grid)
    {
        grid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true);

        var items = grid.ItemsSource as IList;
        if (items == null || items.Count == 0)
        {
            return;
        }

        var currentRowIndex = grid.SelectedIndex < 0 ? 0 : grid.SelectedIndex;
        var nextRowIndex = currentRowIndex + 1;

        if (nextRowIndex >= items.Count)
        {
            return;
        }

        var targetColumn = grid.CurrentColumn;
        var targetItem = items[nextRowIndex];

        grid.SelectedIndex = nextRowIndex;
        if (targetColumn != null)
        {
            grid.CurrentColumn = targetColumn;
            grid.ScrollIntoView(targetItem!, targetColumn);
        }

        if (targetColumn?.IsReadOnly != true)
        {
            Dispatcher.UIThread.Post(() => grid.BeginEdit());
        }
    }

    private static void OnCellGotFocus(DataGridCell cell, FocusChangedEventArgs e)
    {
        if (e.Source != cell)
        {
            return;
        }

        var parentGrid = cell.FindAncestorOfType<DataGrid>();
        if (parentGrid?.IsReadOnly == true)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            var firstFocusable = cell.GetVisualDescendants()
                .OfType<InputElement>()
                .FirstOrDefault(x => x.Focusable && x.IsVisible);
            firstFocusable?.Focus();
        });
    }
}
