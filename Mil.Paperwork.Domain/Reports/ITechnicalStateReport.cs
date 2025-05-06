using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface ITechnicalStateReport : IReport
    {
        bool TryCreate(IAssetInfo assetInfo, DateTime writeOffDate, string reason);
    }
}
