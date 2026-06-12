namespace Mil.Paperwork.Domain.Services
{
    public interface IReportService<T>
    {
        public ReportGenerationResult TryGenerateReport(T reportData);
    }
}
