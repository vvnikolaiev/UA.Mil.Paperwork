using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IWriteOffActReport : IReport
    {
        bool TryCreate(ICommonWriteOffReportData reportParameters);
    }
}
