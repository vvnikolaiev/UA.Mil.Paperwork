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
        // Captured in OnCellEditEnded before DataGrid moves CurrentColumn to target.
        // Used in HandleTabKey (e.Handled=True path) to navigate from the real source column.
        public DataGridColumn? CommittedColumn;
        public int CommittedRowIndex;
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
            // Bubble + handledEventsToo: receives Tab even when Avalonia's keyboard navigation
            // already handled it at Window level (which happens for TextBox descendants).
            grid.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble, handledEventsToo: true);
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
        if (sender is not DataGrid grid || grid.IsReadOnly)
        {
            return;
        }

        // DataGridTemplateColumn without CellEditingTemplate is auto-marked IsReadOnly by Avalonia.
        // Skip only genuinely read-only non-template columns.
        if (e.Column?.IsReadOnly == true && e.Column is not DataGridTemplateColumn)
        {
            return;
        }

        grid.BeginEdit();

        // PreparingCellForEdit never fires for template columns without CellEditingTemplate.
        // Activate the visible cell content directly.
        if (e.Column is DataGridTemplateColumn)
        {
            GetState(grid).IsEditing = true;

            Dispatcher.UIThread.Post(() => UpdateCurrentCell(grid, e.Row.Index, e.Column));
        }
    }

    private static void OnPreparingCellForEdit(object? sender, DataGridPreparingCellForEditEventArgs e)
    {
        if (sender is not DataGrid grid)
        {
            return;
        }

        var state = GetState(grid);
        state.IsEditing = true;

        var editingElement = e.EditingElement;
        var pendingText = state.PendingText;
        state.PendingText = null;

        if (editingElement is TextBox tb)
        {
            if (pendingText != null)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    tb.Focus();
                    tb.Text = pendingText;
                    tb.CaretIndex = pendingText.Length;
                });
            }
            else
            {
                Dispatcher.UIThread.Post(() => tb.Focus());
            }
        }
        else if (editingElement != null)
        {
            Dispatcher.UIThread.Post(() => editingElement.Focus());
        }
    }

    private static void OnCellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
    {
        if (sender is not DataGrid grid)
        {
            return;
        }

        var state = GetState(grid);
        state.IsEditing = false;
        // DataGrid fires CellEditEnded before updating CurrentColumn to the navigation target.
        // Capture here so HandleTabKey can use the real source even after DataGrid has moved.
        state.CommittedColumn = grid.CurrentColumn;
        state.CommittedRowIndex = grid.SelectedIndex;
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
            HandleTabKey(grid, e);
        }
        else if (e.Key == Key.Enter)
        {
            HandleEnterKey(grid, e);
        }
    }

    private static void HandleTabKey(DataGrid grid, KeyEventArgs e)
    {
        CloseOpenDropDowns(grid);
        var backward = (e.KeyModifiers & KeyModifiers.Shift) != 0;

        if (!e.Handled)
        {
            // DataGrid did not navigate (template column auto-IsReadOnly, or text column with no
            // non-readonly successor). CurrentColumn is still the source — navigate ourselves.
            e.Handled = true;
            CommitAndMoveToNextCell(grid, backward, grid.CurrentColumn, grid.SelectedIndex);
        }
        else
        {
            // DataGrid committed the text-column edit and navigated away (possibly skipping
            // template columns). OnCellEditEnded captured the real source before DataGrid moved.
            var state = GetState(grid);
            var sourceColumn = state.CommittedColumn;
            var sourceRowIndex = state.CommittedRowIndex;
            state.CommittedColumn = null;

            e.Handled = true;

            if (sourceColumn != null)
            {
                CommitAndMoveToNextCell(grid, backward, sourceColumn, sourceRowIndex);
            }
            else
            {
                // Text column was selected but not actively edited — DataGrid navigated correctly
                // within text columns. Just activate the inner control where DataGrid landed.
                Dispatcher.UIThread.Post(() => ActivateCurrentCell(grid, grid.SelectedItem, sourceColumn, openDropdowns: false));
            }
        }
    }

    private static void HandleEnterKey(DataGrid grid, KeyEventArgs e)
    {
        CloseOpenDropDowns(grid);

        e.Handled = true;

        var state = GetState(grid);
        if (state.IsEditing || grid.CurrentColumn is DataGridTemplateColumn)
        {
            CommitAndMoveToNextRow(grid);
        }
        else
        {
            grid.BeginEdit();
        }
    }

    private static void CloseOpenDropDowns(DataGrid grid)
    {
        foreach (var comboBox in grid.GetVisualDescendants().OfType<ComboBox>())
        {
            if (comboBox.IsDropDownOpen)
            {
                comboBox.IsDropDownOpen = false;
            }
        }

        foreach (var datePicker in grid.GetVisualDescendants().OfType<CalendarDatePicker>())
        {
            if (datePicker.IsDropDownOpen)
            {
                datePicker.IsDropDownOpen = false;
            }
        }
    }

    private static bool IsAnyDropDownOpen(DataGrid grid)
    {
        foreach (var comboBox in grid.GetVisualDescendants().OfType<ComboBox>())
        {
            if (comboBox.IsDropDownOpen)
            {
                return true;
            }
        }

        foreach (var datePicker in grid.GetVisualDescendants().OfType<CalendarDatePicker>())
        {
            if (datePicker.IsDropDownOpen)
            {
                return true;
            }
        }

        return false;
    }

    private static void CommitAndMoveToNextCell(DataGrid grid, bool backward, DataGridColumn? fromColumn, int fromRowIndex)
    {
        GetState(grid).IsEditing = false;

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

        var currentColIndex = fromColumn != null ? columns.IndexOf(fromColumn) : 0;
        if (currentColIndex < 0)
        {
            currentColIndex = 0;
        }

        var currentRowIndex = fromRowIndex >= 0 ? fromRowIndex : (grid.SelectedIndex >= 0 ? grid.SelectedIndex : 0);

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

        if (targetColumn.IsReadOnly && targetColumn is not DataGridTemplateColumn)
        {
            return;
        }

        if (targetColumn is DataGridTemplateColumn)
        {
            GetState(grid).IsEditing = true;
            Dispatcher.UIThread.Post(() => ActivateCurrentCell(grid, targetItem, targetColumn, openDropdowns: false));
        }
        else
        {
            Dispatcher.UIThread.Post(() => grid.BeginEdit());
        }
    }

    private static void CommitAndMoveToNextRow(DataGrid grid)
    {
        GetState(grid).IsEditing = false;

        var items = grid.ItemsSource as IList;
        if (items == null || items.Count == 0)
        {
            return;
        }

        var currentRowIndex = grid.SelectedIndex < 0 ? 0 : grid.SelectedIndex;
        var nextRowIndex = currentRowIndex; // already updated after "Enter".

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

        if (targetColumn?.IsReadOnly == true && targetColumn is not DataGridTemplateColumn)
        {
            return;
        }

        if (targetColumn is DataGridTemplateColumn)
        {
            GetState(grid).IsEditing = true;
            Dispatcher.UIThread.Post(() => ActivateCurrentCell(grid, targetItem, targetColumn, openDropdowns: false));
        }
        else
        {
            Dispatcher.UIThread.Post(() => grid.BeginEdit());
        }
    }

    private static void UpdateCurrentCell(DataGrid grid, int rowIndex, DataGridColumn column)
    {
        if (rowIndex < 0 || column == null)
        {
            return;
        }

        if (grid.SelectedIndex != rowIndex)
        {
            grid.SelectedIndex = rowIndex;
        }

        grid.CurrentColumn = column;
    }

    //private static void ActivateCurrentCell(DataGrid grid, bool openDropdowns = false)
    private static void ActivateCurrentCell(DataGrid grid, object? selectedItem, DataGridColumn currentColumn, bool openDropdowns = false)
    {
        //var selectedItem = grid.SelectedItem;
        //var currentColumn = grid.CurrentColumn;
        if (selectedItem == null || currentColumn == null)
        {
            return;
        }

        var columnIndex = currentColumn.DisplayIndex;// grid.Columns.IndexOf(currentColumn);
        if (columnIndex < 0)
        {
            return;
        }

        var row = grid.GetVisualDescendants()
            .OfType<DataGridRow>()
            .FirstOrDefault(r => r.DataContext == selectedItem);

        if (row == null)
        {
            return;
        }

        var cells = row.GetVisualDescendants().OfType<DataGridCell>().ToList();
        if (columnIndex < cells.Count)
        {
            ActivateCellByContent(cells[columnIndex], openDropdowns);
        }

        if (grid.SelectedIndex != row.Index)
        {
            grid.SelectedIndex = row.Index;
        }

        grid.CurrentColumn = currentColumn;
    }

    private static void ActivateCellByContent(DataGridCell cell, bool openDropdowns = false)
    {
        //return;
        var numericUpDown = cell.GetVisualDescendants().OfType<NumericUpDown>().FirstOrDefault();
        if (numericUpDown != null)
        {
            numericUpDown.Focus();
            return;
        }

        var comboBox = cell.GetVisualDescendants().OfType<ComboBox>().FirstOrDefault();
        if (comboBox != null && !comboBox.IsFocused)
        {
            comboBox.Focus();
            if (openDropdowns)
            {
                comboBox.IsDropDownOpen = true;
            }
            return;
        }

        var datePicker = cell.GetVisualDescendants().OfType<CalendarDatePicker>().FirstOrDefault();
        if (datePicker != null)
        {
            datePicker.Focus();
            return;
        }

        var firstFocusable = cell.GetVisualDescendants()
            .OfType<InputElement>()
            .FirstOrDefault(x => x.Focusable && x.IsVisible);
        firstFocusable?.Focus();
    }

    private static void OnCellGotFocus(DataGridCell cell, FocusChangedEventArgs e)
    {
        // Only handle when the DataGridCell itself receives focus (arrow-key navigation,
        // DataGrid-internal focus moves after BeginEdit). When a descendant (TextBox, NumericUpDown,
        // ComboBox) already has focus, e.Source != cell — skip to avoid double-activation.
        if (e.Source != cell)
        {
            return;
        }

        var parentGrid = cell.FindAncestorOfType<DataGrid>();
        if (parentGrid == null || parentGrid.IsReadOnly)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (parentGrid.CurrentColumn is DataGridTemplateColumn)
            {
                GetState(parentGrid).IsEditing = true;
                //ActivateCellByContent(cell, openDropdowns: false);
            }
            else
            {
                GetState(parentGrid).IsEditing = false;
                var firstFocusable = cell.GetVisualDescendants()
                    .OfType<InputElement>()
                    .FirstOrDefault(x => x.Focusable && x.IsVisible);
                firstFocusable?.Focus();
            }
        });
    }
}
