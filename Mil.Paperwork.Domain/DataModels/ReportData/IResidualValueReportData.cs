using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface  IResidualValueReportData : IReportData
    {
        AssetType AssetType { get; set; }
        IList<IAssetInfo> Assets { get; set; }
        IDictionary<MetalType, decimal> MetalCosts { get; set; }
        DateTime EventDate { get; set; }
        int? EventReportNumber { get; set; }
    }
}
