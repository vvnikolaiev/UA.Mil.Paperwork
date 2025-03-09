using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Services
{
    public interface IReportService
    {
        public bool TryGenerateReport(WriteOffReportData reportData);
    }
}
