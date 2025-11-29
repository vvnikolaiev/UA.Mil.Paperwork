using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.UI.ViewModels.Assets
{
    public abstract class WriteOffAssetViewModel : AssetViewModel
    {
        protected EventType eventType = EventType.Lost;
        public EventType EventType
        {
            get => eventType;
            set => SetProperty(ref eventType, value);
        }

        public virtual IAssetInfo ToAssetInfo(EventType eventType = EventType.None)
        {
            var assetInfo = base.ToAssetInfo();
            assetInfo.EventType = eventType;

            return assetInfo;
        }
    }
}
