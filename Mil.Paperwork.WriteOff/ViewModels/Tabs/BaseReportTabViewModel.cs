using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.MVVM;
using System.Windows;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal abstract class BaseReportTabViewModel : ObservableItem, IReportTabViewModel
    {
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public event EventHandler<ReportType> OpenReportSettingsRequested;

        public abstract string Header { get; }

        protected void Close()
        {
            if (MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
            }
        }

        protected void OpenSettings(ReportType reportType)
        {
            OpenReportSettingsRequested?.Invoke(this, reportType);
        }
    }
}