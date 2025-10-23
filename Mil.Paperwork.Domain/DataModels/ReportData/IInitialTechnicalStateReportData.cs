using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IInitialTechnicalStateReportData : IBaseTechnicalStateReportData
    {
        IPerson PersonAccepted { get; }
        IPerson PersonHanded { get; }
    }
}