using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public interface IDismantlingReportData : IAssetValuationReportData
    {
        IList<AssetDismantlingData> Dismantlings { get; }
    }
}
