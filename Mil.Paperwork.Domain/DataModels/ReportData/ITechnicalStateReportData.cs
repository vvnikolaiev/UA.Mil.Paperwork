using Mil.Paperwork.Domain.DataModels.Parameters;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface ITechnicalStateReportData : IBaseTechnicalStateReportData
    {
        DateTime DocumentDate { get; }

        string Reason { get; }

        DateTime EventDate { get; }

        int OrdenNumber { get; }

        DateTime OrdenDate { get; }

        bool GenerateWriteOffActs { get; }
    }
}
