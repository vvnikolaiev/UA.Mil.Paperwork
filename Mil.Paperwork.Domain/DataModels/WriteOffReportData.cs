using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;
using System.Security.Policy;

namespace Mil.Paperwork.Domain.DataModels
{
    public class WriteOffReportData : ITechnicalStateReportData, IDismantlingReportData, IAssetValuationReportData
    {
        public int? EventReportNumber { get; set; }

        public AssetType AssetType { get; set; }

        public EventType EventType { get; set; }

        public string DestinationFolder { get; set; }

        public string RegistrationNumber { get; set; }

        public string DocumentNumber { get; set; }

        public string Reason { get; set; }
        
        public DateTime ReportDate { get; set; }

        public IList<IAssetInfo> Assets { get; set; }

        public IList<AssetDismantlingData> Dismantlings { get; set; }

        public IList<IAssetValuationData?> ValuationData { get; set; }

        public string GetDestinationPath()
        {
            var destinationPath = PathsHelper.GetDestinationPath(DestinationFolder, ReportDate);
            return destinationPath;
        }

    }
}
