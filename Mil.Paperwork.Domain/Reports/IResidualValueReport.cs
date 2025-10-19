using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IResidualValueReport : IReport
    {
        bool TryCreate(IAssetInfo asset, IDictionary<MetalType, decimal> metalCosts, AssetType assetType, DateTime reportDate);
    }
}
