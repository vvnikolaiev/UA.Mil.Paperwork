using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal abstract class BaseReportTabViewModel : BaseTabViewModel, IReportTabViewModel
    {
        public event EventHandler<ReportType> OpenReportSettingsRequested;

        public BaseReportTabViewModel(IDialogService dialogService) : base(dialogService)
        {
        }

        protected void OpenSettings(ReportType reportType)
        {
            OpenReportSettingsRequested?.Invoke(this, reportType);
        }
    }
}