using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class ResidualValueReportData : IResidualValueReportData
    {
        public AssetType AssetType { get; set; }
        public IList<IAssetInfo> Assets { get; set; }
        public IDictionary<MetalType, decimal> MetalCosts { get; set; }
        public DateTime EventDate { get; set; }
        public string DestinationFolder { get; set; }
        public int? EventReportNumber { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
