using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IAssetValuationReport : IReport
    {
        bool TryCreate(IAssetValuationData assetValuationData);
    }
}