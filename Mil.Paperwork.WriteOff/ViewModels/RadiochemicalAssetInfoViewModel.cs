using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class RadiochemicalAssetInfoViewModel : WriteOffAssetViewModel
    {
        private readonly RadiochemicalAssetInfo _assetInfo;

        private bool _isLocal = true;

        // for future iplementation of IDataGridColumnProvider using Source Generator?
        //[DataGridColumn("Local?", ColumnType.CheckBox)]
        public bool IsLocal
        {
            get => _isLocal;
            set => SetProperty(ref _isLocal, value);
        }

        internal override IAssetInfo AssetInfo => _assetInfo;

        public RadiochemicalAssetInfoViewModel() : base()
        {
            _assetInfo = new RadiochemicalAssetInfo();
        }

        public override IAssetInfo ToAssetInfo(EventType eventType = EventType.None)
        {
            base.ToAssetInfo(eventType);
            _assetInfo.IsLocal = IsLocal;

            return _assetInfo;
        }
    }
}
