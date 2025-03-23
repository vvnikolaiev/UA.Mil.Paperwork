using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public class DismantlingReportData : IDismantlingReportData
    {
        public string DestinationFolder { get; set; }

        public IList<AssetDismantlingData> Dismantlings { get; set; }

        public IList<IAssetValuationData?> ValuationData => [.. Dismantlings.Cast<IAssetValuationData>()];

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }
    }
}
