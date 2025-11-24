using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.UI.ViewModels.Assets
{
    public class InvoiceAssetViewModel : AssetViewModel
    {
        private IAssetInfo _assetInfo;

        internal override IAssetInfo AssetInfo => _assetInfo;

        public InvoiceAssetViewModel() : base()
        {
            _assetInfo = new AssetInfo();
        }
    }
}
