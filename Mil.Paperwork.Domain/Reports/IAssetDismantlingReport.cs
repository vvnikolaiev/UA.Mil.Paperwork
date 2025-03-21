using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IAssetDismantlingReport : IReport
    {
        bool TryCreate(AssetDismantlingData assetDismantlingData);
    }
}
