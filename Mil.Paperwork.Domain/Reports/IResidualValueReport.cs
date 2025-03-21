using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IResidualValueReport : IReport
    {
        bool TryCreate(WriteOffReportData reportData);
    }
}
