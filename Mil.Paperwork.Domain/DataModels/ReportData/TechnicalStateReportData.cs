namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class TechnicalStateReportData : InitialTechnicalStateReportData, ITechnicalStateReportData
    {
        public string Reason { get; set; }

        public DateTime ReportDate { get; set; }
    }
}
