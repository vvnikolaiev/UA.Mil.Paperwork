using Mil.Paperwork.Domain.Calculators;
using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class ResidualPriceHelper
    {
        public static decimal CalculateTotalReportSum(IEnumerable<IAssetInfo> assets, DateTime reportDate, bool withCoefficients)
        {
            decimal totalSum = 0;
            foreach (var asset in assets)
            {
                var priceForItem = asset.Price;
                if (withCoefficients)
                {
                    priceForItem = CalculateResidualPriceForItem(asset, reportDate);
                }


                totalSum += Math.Round(asset.Count * priceForItem, 2);
            }
            return totalSum;
        }
        
        public static decimal CalculateResidualPriceForItem(IAssetInfo asset, DateTime reportDate, int count = 1)
        {
            var calculator = ResidualPriceCalculatorFactory.CreateCalculator(asset.Service);
            
            var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate.Year, reportDate.Year);

            var totalWearCoeff = calculator.CalculateTotalWearCoefficient(asset, reportDate);
            var result = CalculateResidualPrice(asset.Price, indexationCoefficient, totalWearCoeff, count);

            return result;
        }

        public static decimal CalculateResidualPrice(decimal price, decimal indexationCoeff, decimal skzCoeff, int count)
        {
            var indexatedValue = Math.Round(price * indexationCoeff, 2);
            var result = Math.Round(indexatedValue * skzCoeff, 2) * count;

            return result;
        }
    }
}
