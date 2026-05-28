using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class CommonWriteOffReportData : ICommonWriteOffReportData
    {
        public string RegistrationNumber { get; set; }

        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }

        public string Reason { get; set; }

        public DateTime EventDate { get; set; }

        public int OrdenNumber { get; set; }

        public DateTime OrdenDate { get; set; }

        public EventType EventType { get; set; }

        public IList<IAssetInfo> Assets { get; set; }

        public string DestinationFolder { get; set; }

        public static CommonWriteOffReportData FromReportData(ICommonWriteOffReportData reportData)
        {
            return new CommonWriteOffReportData
            {
                DocumentDate = reportData.DocumentDate,
                EventType = reportData.EventType,
                EventDate = reportData.EventDate,
                OrdenNumber = reportData.OrdenNumber,
                OrdenDate = reportData.OrdenDate,
                Reason = reportData.Reason,
                Assets = reportData.Assets,
            };
        }

        public string GetDestinationPath()
        {
            return DestinationFolder;
        }

    }
}
