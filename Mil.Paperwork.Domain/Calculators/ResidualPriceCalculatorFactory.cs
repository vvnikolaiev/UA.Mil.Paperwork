using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.Calculators
{
    internal static class ResidualPriceCalculatorFactory
    {
        public static IResidualPriceCalculator CreateCalculator(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Connectivity => new ConnectivityResidualPriceCalculator(),
                AssetType.Radiochemical => new RadiochemicalResidualPriceCalculator(),
                _ => new DefaultResidualPriceCalculator(),
            };
        }
    }
}
