using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.UI.ViewModels.Assets
{
    internal class ConnectivityAssetInfoViewModel : WriteOffAssetViewModel
    {
        private readonly ConnectivityAssetInfo _assetInfo;
        
        private decimal _wearAndTearCoeff = 0.8m;

        //[DataGridColumn("Kwt")]
        public decimal WearAndTearCoeff
        {
            get => _wearAndTearCoeff;
            set => SetProperty(ref _wearAndTearCoeff, value);
        }

        internal override IAssetInfo AssetInfo => _assetInfo;

        public ConnectivityAssetInfoViewModel() : base()
        {
            _assetInfo = new ConnectivityAssetInfo();
        }

        public override IAssetInfo ToAssetInfo(EventType eventType = EventType.None)
        {
            base.ToAssetInfo(eventType);
            _assetInfo.WearAndTearCoeff = _wearAndTearCoeff;

            return _assetInfo;
        }
    }
}
