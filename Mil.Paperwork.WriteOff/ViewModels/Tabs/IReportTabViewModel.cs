using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal interface IReportTabViewModel : ITabViewModel
    {
        event EventHandler<ReportType> OpenReportSettingsRequested;
    }
}
