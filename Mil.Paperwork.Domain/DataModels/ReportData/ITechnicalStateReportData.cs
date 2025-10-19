using Mil.Paperwork.Domain.DataModels.Parameters;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface ITechnicalStateReportData : IInitialTechnicalStateReportData
    {
        DateTime DocumentDate { get; }

        string Reason { get; }

        DateTime EventDate { get; }

        int OrdenNumber { get; }

        DateTime OrdenDate { get; }

        bool GenerateWriteOffActs { get; }

        // TODO: move into a separate ReportData interface?
        IBookExtractData BookOfLossesExtractData { get; }
    }
}
