using Mil.Paperwork.Infrastructure.Enums;

namespace Mil.Paperwork.Domain.Calculators
{
    internal static class ResidualPriceCalculatorFactory
    {
        public static IResidualPriceCalculator CreateCalculator(AssetType assetType) => assetType switch
        {
            AssetType.Connectivity          => new ConnectivityResidualPriceCalculator(),
            AssetType.Radiochemical         => new RadiochemicalResidualPriceCalculator(),
            AssetType.MissileAirDefense     => new MissileAirDefenseResidualPriceCalculator(),
            AssetType.Artillery             => new ArtilleryResidualPriceCalculator(),
            AssetType.SmallArms             => new SmallArmsResidualPriceCalculator(),
            AssetType.Ammunition            => new AmmunitionResidualPriceCalculator(),
            AssetType.ArmoredVehicles       => new ArmoredVehiclesResidualPriceCalculator(),
            AssetType.Engineering           => new EngineeringResidualPriceCalculator(),
            AssetType.EngineeringAmmunition => new EngineeringAmmunitionResidualPriceCalculator(),
            AssetType.AutomotiveProperty    => new AutomotivePropertyResidualPriceCalculator(),
            AssetType.Topographic           => new TopographicResidualPriceCalculator(),
            AssetType.MeasuringEquipment    => new MeasuringEquipmentResidualPriceCalculator(),
            AssetType.FoodService           => new FoodServiceResidualPriceCalculator(),
            AssetType.ElectronicWarfare     => new ElectronicWarfareResidualPriceCalculator(),
            AssetType.UAV                   => new UAVResidualPriceCalculator(),
            _                               => new DefaultResidualPriceCalculator()
        };
    }
}
