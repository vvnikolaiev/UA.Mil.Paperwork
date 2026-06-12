using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.UI.ViewModels.Tabs
{
    internal interface IReportDataLoadable<in TData> where TData : IReportData
    {
        void LoadReportData(TData data);
    }
}
