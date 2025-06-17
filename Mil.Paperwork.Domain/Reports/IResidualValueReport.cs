using Mil.Paperwork.Domain.DataModels.ReportData;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IResidualValueReport : IReport
    {
        bool TryCreate(WriteOffReportData reportData);
    }
}
