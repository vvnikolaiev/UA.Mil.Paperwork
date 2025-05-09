using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.Reports
{
    public interface ITechnicalStateReport : IReport
    {
        bool TryCreate(IAssetInfo assetInfo, EventType eventType, DateTime writeOffDate, string reason);
    }
}
