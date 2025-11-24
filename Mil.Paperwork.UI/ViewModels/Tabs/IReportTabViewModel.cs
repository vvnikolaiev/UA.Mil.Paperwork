using Mil.Paperwork.Infrastructure.Enums;
using System;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal interface IReportTabViewModel : ITabViewModel
    {
        event EventHandler<ReportType> OpenReportSettingsRequested;
    }
}
