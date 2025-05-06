using Mil.Paperwork.Domain.Services;
using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.Managers;
using Mil.Paperwork.WriteOff.ViewModels;

namespace Mil.Paperwork.WriteOff.Factories
{
    internal class AssetFactory : IAssetFactory
    {
        private readonly IReportDataService _reportDataService;
        private readonly ReportManager _reportManager;
        private readonly IDataService _dataService;
        private readonly INavigationService _navigationService;

        public AssetFactory(
            IReportDataService reportDataService,
            ReportManager reportManager,
            IDataService dataService,
            INavigationService navigationService)
        {
            _reportDataService = reportDataService;
            _reportManager = reportManager;
            _dataService = dataService;
            _navigationService = navigationService;
        }

        public AssetViewModel CreateAssetViewModel()
        {
            var assetType = _reportDataService.GetAssetType();

            return assetType switch
            {
                AssetType.Connectivity => new ConnectivityAssetInfoViewModel(_reportManager, _dataService, _navigationService),
                AssetType.Radiochemical => new RadiochemicalAssetInfoViewModel(_reportManager, _dataService, _navigationService),
                _ => throw new ArgumentException("Invalid asset type", nameof(assetType)),
            };

        }
    }
}
