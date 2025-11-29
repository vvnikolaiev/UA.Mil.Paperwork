using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class DefaultAssetInfoViewModel : WriteOffAssetViewModel
    {
        private readonly AssetInfo _assetInfo;

        internal override IAssetInfo AssetInfo => _assetInfo;

        public DefaultAssetInfoViewModel() : base()
        {
            _assetInfo = new AssetInfo();
        }

        public override IAssetInfo ToAssetInfo(EventType eventType = EventType.None)
        {
            base.ToAssetInfo(eventType);
            return _assetInfo;
        }
    }
}
