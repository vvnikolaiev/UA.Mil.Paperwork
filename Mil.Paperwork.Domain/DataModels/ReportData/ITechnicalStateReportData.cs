namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface ITechnicalStateReportData : IInitialTechnicalStateReportData
    {
        string Reason { get; }

        DateTime ReportDate { get; }
    }
}
