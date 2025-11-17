using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Common.MVVM;
using Mil.Paperwork.Infrastructure.Services;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal abstract class BaseReportTabViewModel : ObservableItem, IReportTabViewModel
    {
        private readonly IDialogService _dialogService;

        public event EventHandler<ITabViewModel> TabCloseRequested;

        public event EventHandler<ReportType> OpenReportSettingsRequested;

        public abstract string Header { get; }

        public BaseReportTabViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        protected void Close()
        {
            var dlgResult = _dialogService.ShowMessage("Are you sure you want to close this tab?", "Confirmation", DialogButtons.YesNo);
            if (dlgResult == DialogResult.Yes)
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