using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IInitialTechnicalStateReportData : IReportData
    {
        EventType EventType { get; }

        IList<IAssetInfo> Assets { get; set; }
    }
}
