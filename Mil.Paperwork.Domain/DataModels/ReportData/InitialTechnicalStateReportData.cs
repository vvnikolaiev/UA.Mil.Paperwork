using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public class InitialTechnicalStateReportData : BaseTechnicalStateReportData, IInitialTechnicalStateReportData
    {

        public IPerson PersonAccepted { get; set; }
        public IPerson PersonHanded { get; set; }
    }
}
