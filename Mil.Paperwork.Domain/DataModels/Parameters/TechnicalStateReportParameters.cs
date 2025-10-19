using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.Parameters
{
    internal struct TechnicalStateReportParameters : ITechnicalStateReportParameters
    {
        public IAssetInfo AssetInfo { get; set; }
        public EventType EventType { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EventDate { get; set; }
        public int OrdenNumber { get; set; }
        public DateTime OrdenDate { get; set; }
        public string Reason { get; set; }

        public static TechnicalStateReportParameters FromReportData(ITechnicalStateReportData reportData)
        {
            return new TechnicalStateReportParameters
            {
                DocumentDate = reportData.DocumentDate,
                EventType = reportData.EventType,
                EventDate = reportData.EventDate,
                OrdenNumber = reportData.OrdenNumber,
                OrdenDate = reportData.OrdenDate,
                Reason = reportData.Reason
            };
        }
    }
}
