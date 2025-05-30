using Mil.Paperwork.WriteOff.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels.Tabs
{
    internal interface IReportTabViewModel : ITabViewModel
    {
        event EventHandler<SettingsTabType> SettingsTabOpenRequested;
    }
}
