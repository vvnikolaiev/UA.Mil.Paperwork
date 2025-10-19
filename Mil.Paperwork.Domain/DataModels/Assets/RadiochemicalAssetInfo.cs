using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class RadiochemicalAssetInfo : AssetInfo
    {
        public override AssetType Service => AssetType.Radiochemical;
        // if from USA then different coefficient
        public bool IsLocal { get; set; } = true;
    }
}