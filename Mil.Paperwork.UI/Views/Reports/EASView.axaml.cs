using Avalonia.Controls;
using Mil.Paperwork.UI.ViewModels.Reports;

namespace Mil.Paperwork.UI.Views.Reports;

public partial class EASView : UserControl
{
    public EASView()
    {
        InitializeComponent();
    }

    private void OnRestrictedDateChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is not EASViewModel vm)
        {
            return;
        }

        if (sender is not CalendarDatePicker picker)
        {
            return;
        }

        if (picker.SelectedDate.HasValue && picker.SelectedDate.Value.Date > vm.ReportDate.Date)
        {
            picker.SelectedDate = vm.ReportDate;
        }
    }
}
