using Mil.Paperwork.Infrastructure.Enums;
using Mil.Paperwork.Infrastructure.Services;
using Mil.Paperwork.WriteOff.ViewModels;

namespace Mil.Paperwork.WriteOff.Factories
{
    internal class AssetFactory : IAssetFactory
    {
        private readonly IReportDataService _reportDataService;

        public AssetFactory(IReportDataService reportDataService)
        {
            _reportDataService = reportDataService;
        }

        public WriteOffAssetViewModel CreateAssetViewModel()
        {
            var assetType = _reportDataService.GetAssetType();

            return assetType switch
            {
                AssetType.Connectivity => new ConnectivityAssetInfoViewModel(),
                AssetType.Radiochemical => new RadiochemicalAssetInfoViewModel(),
                AssetType.Default => new DefaultAssetInfoViewModel(),
                _ => throw new ArgumentException("Invalid asset type", nameof(assetType)),
            };

        }
    }
}
