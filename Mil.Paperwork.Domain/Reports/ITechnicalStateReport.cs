using Mil.Paperwork.Domain.DataModels.Parameters;

namespace Mil.Paperwork.Domain.Reports
{
    public interface ITechnicalStateReport : IReport
    {
        bool TryCreate(ITechnicalStateReportParameters reportParameters);
    }
}
