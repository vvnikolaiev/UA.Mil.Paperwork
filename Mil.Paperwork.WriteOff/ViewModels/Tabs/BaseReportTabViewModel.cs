using Mil.Paperwork.Infrastructure.MVVM;
using Mil.Paperwork.WriteOff.Enums;
using System.Windows;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal abstract class BaseReportTabViewModel : ObservableItem, IReportTabViewModel
    {
        public event EventHandler<ITabViewModel> TabCloseRequested;

        public event EventHandler<SettingsTabType> SettingsTabOpenRequested;

        public abstract string Header { get; }

        protected void Close()
        {
            if (MessageBox.Show("Are you sure you want to close this tab?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                TabCloseRequested.Invoke(this, this);
            }
        }

        protected void OpenSettings(SettingsTabType settingsTabType)
        {
            SettingsTabOpenRequested?.Invoke(this, settingsTabType);
        }
    }
}