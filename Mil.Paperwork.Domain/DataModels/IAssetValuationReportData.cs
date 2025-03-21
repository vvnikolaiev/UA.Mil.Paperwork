using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public interface IAssetValuationReportData : IReportData
    {
        IList<IAssetValuationData> ValuationData { get; }
    }
}
