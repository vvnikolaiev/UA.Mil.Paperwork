using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IInitialTechnicalStateReport : IReport
    {
        bool TryCreate(IAssetInfo assetInfo, EventType eventType);
    }
}
