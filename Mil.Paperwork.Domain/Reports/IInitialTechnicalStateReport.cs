using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.Reports
{
    public interface IInitialTechnicalStateReport : IReport
    {
        bool TryCreate(IAssetInfo assetInfo, IPerson PersonAccepted, IPerson PersonHanded, EventType eventType);
    }
}
