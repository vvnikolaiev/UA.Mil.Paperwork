using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    public class ConnectivityAssetInfoViewModel : AssetViewModel
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

        public ConnectivityAssetInfoViewModel(
            ReportManager reportManager,
            IDataService dataService,
            INavigationService navigationService) : base(reportManager, dataService, navigationService)
        {
            _assetInfo = new ConnectivityAssetInfo();
        }

        public override IAssetInfo ToAssetInfo(DateTime? reportDate = null)
        {
            base.ToAssetInfo(reportDate);
            _assetInfo.WearAndTearCoeff = _wearAndTearCoeff;

            return _assetInfo;
        }
    }
}
