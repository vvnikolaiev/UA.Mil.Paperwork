namespace Mil.Paperwork.Domain.DataModels
{
    public class TechnicalStateReportData : InitialTechnicalStateReportData, ITechnicalStateReportData
    {
        public string Reason { get; set; }

        public DateTime ReportDate { get; set; }
    }
}
