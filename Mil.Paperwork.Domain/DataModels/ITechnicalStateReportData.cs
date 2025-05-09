using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels
{
    public interface ITechnicalStateReportData : IReportData
    {
        string Reason { get; }

        DateTime ReportDate { get; }

        EventType EventType { get; }

        IList<IAssetInfo> Assets { get; set; }
    }
}
