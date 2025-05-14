using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Enums;
using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;

namespace Mil.Paperwork.WriteOff.ViewModels
{
    internal class ConnectivityAssetInfoViewModel : AssetViewModel
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

        public override IAssetInfo ToAssetInfo(EventType eventType = EventType.Lost, DateTime ? reportDate = null)
        {
            base.ToAssetInfo(eventType, reportDate);
            _assetInfo.WearAndTearCoeff = _wearAndTearCoeff;

            return _assetInfo;
        }
    }
}
