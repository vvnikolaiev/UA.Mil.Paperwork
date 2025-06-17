using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class AssetValuationReportData : IAssetValuationReportData
    {
        public string DestinationFolder { get; set; }

        public IList<IAssetValuationData?> ValuationData { get; set; }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
