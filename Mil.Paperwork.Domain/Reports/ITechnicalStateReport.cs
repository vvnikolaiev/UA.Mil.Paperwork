using Mil.Paperwork.Domain.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface ITechnicalStateReport : IReport
    {
        bool TryCreate(AssetInfo assetInfo, DateTime writeOffDate, string reason);
    }
}
