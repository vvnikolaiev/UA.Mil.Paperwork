using Mil.Paperwork.Domain.DataModels.Parameters;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class TechnicalStateReportData : BaseTechnicalStateReportData, ITechnicalStateReportData
    {
        public DateTime DocumentDate { get; set; }

        public string Reason { get; set; }

        public DateTime EventDate { get; set; }

        public int OrdenNumber { get; set; }

        public DateTime OrdenDate { get; set; }

        public bool GenerateWriteOffActs { get; set; } = true;

        public IBookExtractData? BookOfLossesExtractData { get; set; }

    }
}
