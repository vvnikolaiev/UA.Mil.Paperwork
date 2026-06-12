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

        internal static InvoiceAssetViewModel FromAssetInfo(IAssetInfo assetInfo)
        {
            var vm = new InvoiceAssetViewModel();
            vm.LoadFrom(assetInfo);
            return vm;
        }
    }
}
