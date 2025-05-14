namespace Mil.Paperwork.Domain.DataModels
{
    public interface ITechnicalStateReportData : IInitialTechnicalStateReportData
    {
        string Reason { get; }

        DateTime ReportDate { get; }
    }
}
