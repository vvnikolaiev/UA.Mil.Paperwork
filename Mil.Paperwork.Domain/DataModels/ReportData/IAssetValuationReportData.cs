using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels.ReportData
{
    public interface IAssetValuationReportData : IReportData
    {
        IList<IAssetValuationData?> ValuationData { get; }
    }
}
