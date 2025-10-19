using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    [Obsolete]
    public class ObsoleteWriteOffReportData : ITechnicalStateReportData, IDismantlingReportData, IAssetValuationReportData, IResidualValueReportData, IQualityStateReportData
    {
        public int? EventReportNumber { get; set; }

        public AssetType AssetType { get; set; }

        public EventType EventType { get; set; }

        public string DestinationFolder { get; set; }

        public string RegistrationNumber { get; set; }

        public string DocumentNumber { get; set; }

        public string Reason { get; set; }
        
        public DateTime DocumentDate { get; set; } = DateTime.Now.Date;

        public DateTime EventDate { get; set; }

        public int OrdenNumber { get; set; }

        public DateTime OrdenDate { get; set; }

        public IList<IAssetInfo> Assets { get; set; }

        public IDictionary<MetalType, decimal> MetalCosts { get; set; }
        public IList<AssetDismantlingData> Dismantlings { get; set; }

        public IList<IAssetValuationData?> ValuationData { get; set; }

        public bool GenerateWriteOffActs { get; set; } = true;

        public IBookExtractData BookOfLossesExtractData { get; set; }

        public string GetDestinationPath()
        {
            var destinationPath = PathsHelper.GetDestinationPath(DestinationFolder, EventReportNumber, EventDate);
            return destinationPath;
        }

    }
}
