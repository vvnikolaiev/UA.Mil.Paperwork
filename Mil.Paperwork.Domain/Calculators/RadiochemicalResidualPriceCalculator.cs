using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Domain.Calculators
{
    internal class RadiochemicalResidualPriceCalculator : DefaultResidualPriceCalculator
    {
        // категорія + сша/наше
        public override decimal CalculateTotalWearCoefficient(IAssetInfo asset, DateTime reportDate)
        {
            var category = ReportHelper.ConvertEventTypeToCategory(asset.InitialCategory, asset.EventType);

            // TODO: figure out how to do it the right way
            var isLocal = true;
            if (asset is RadiochemicalAssetInfo rcAsset)
            {
                isLocal = rcAsset.IsLocal;
            }

            decimal coeffSKZ = (category, isLocal) switch
            {
                (1, true) => 1,
                (1, false) => 1,
                (2, true) => 0.8m,
                (2, false) => 0.8m,
                (3, true) => 0.6m,
                (3, false) => 0.6m,
                (4, true) => 0.4m,
                (4, false) => 0.4m,
                (5, true) => 0.3m,
                (5, false) => 0.2m,
                _ => throw new NotImplementedException()
            };

            return coeffSKZ;
        }
    }
}
