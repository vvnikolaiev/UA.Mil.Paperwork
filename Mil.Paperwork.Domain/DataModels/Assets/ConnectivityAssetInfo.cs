using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class ConnectivityAssetInfo : AssetInfo
    {
        public override AssetType Service => AssetType.Connectivity;
        
        public decimal WearAndTearCoeff { get; set; } = 0.8m;

    }
}
