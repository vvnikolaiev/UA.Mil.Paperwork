using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IDismantlingReportData : IAssetValuationReportData
    {
        IList<AssetDismantlingData> Dismantlings { get; }
    }
}
