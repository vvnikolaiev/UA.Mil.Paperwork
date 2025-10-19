using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Domain.Calculators
{
    internal class ConnectivityResidualPriceCalculator : IResidualPriceCalculator
    {
        private const decimal DefaultWearAndTearCoeff = 0.8m; // get from storage conditions

        public IList<decimal> GetCoefficients(IAssetInfo asset, DateTime reportDate)
        {
            // TODO: figure out how to do it the right way
            var wearAndTearCoeff = DefaultWearAndTearCoeff;
            if (asset is ConnectivityAssetInfo conAsset)
            {
                wearAndTearCoeff = conAsset.WearAndTearCoeff;
            }

            var category = ReportHelper.ConvertEventTypeToCategory(asset.InitialCategory, asset.EventType);

            var explotationCoefficient = CoefficientsHelper.GetExploitationCoefficient(asset.StartDate, reportDate);
            var workCoefficient = GetWorkCoefficient(asset, reportDate);
            var technicalStateCoefficient = CoefficientsHelper.GetTechnicalStateCoefficient(category);

            var coeficients = new List<decimal>() { explotationCoefficient, workCoefficient, wearAndTearCoeff, technicalStateCoefficient };
            return coeficients;
        }

        public decimal CalculateTotalWearCoefficient(IAssetInfo asset, DateTime reportDate)
        {
            var coeffs = GetCoefficients(asset, reportDate);
            var coeffSKZ = coeffs.Aggregate(1.0m, (acc, val) => acc * val);
            return coeffSKZ;
        }

        private decimal GetWorkCoefficient(IAssetInfo asset, DateTime reportDate)
        {
            var startDate = asset.YearManufactured > 1900 && asset.YearManufactured <= asset.StartDate.Year
                               ? new DateTime(asset.YearManufactured, 1, 1)
                               : asset.StartDate;

            var workCoefficient = CoefficientsHelper.GetWorkCoefficient(startDate, reportDate, asset.ResourceYears);
            return workCoefficient;
        }
    }
}
