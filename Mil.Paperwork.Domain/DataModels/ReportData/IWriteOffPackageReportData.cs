using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.DataModels.Parameters;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IWriteOffPackageReportData : IReportData
    {
        IList<IAssetInfo> Assets { get; set; }
        DateTime DocumentDate { get; }
        DateTime EventDate { get; }
        int OrdenNumber { get; }
        DateTime OrdenDate { get; }
        IBookExtractData BookOfLossesExtractData { get; }
        string ServiceKey { get; set; }
    }
}
