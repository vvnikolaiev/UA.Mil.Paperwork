namespace Mil.Paperwork.Domain.Services
{
    public interface IReportService<T>
    {
        public bool TryGenerateReport(T reportData);
    }
}
