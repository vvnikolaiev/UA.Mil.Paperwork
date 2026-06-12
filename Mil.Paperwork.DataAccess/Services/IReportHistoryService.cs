using Mil.Paperwork.Domain.DataModels.ReportData;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.DataAccess.Services
{
    public interface IReportHistoryService
    {
        Guid SaveDraft(ReportType reportType, IReportData reportData, Guid? entryId);
        Guid SaveGenerated(ReportType reportType, IReportData reportData, IReadOnlyList<string> generatedFiles, Guid? entryId);
    }
}
