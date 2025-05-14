using Mil.Paperwork.Domain.DataModels;
using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.Domain.Helpers
{
    public static class ResidualPriceHelper
    {
        public static decimal CalculateTotalReportSum(WriteOffReportData reportData, bool withCoefficients)
        {
            decimal totalSum = 0;
            foreach (var asset in reportData.Assets)
            {
                var priceForItem = asset.Price;
                if (withCoefficients)
                {
                    priceForItem = CalculateResidualPriceForItem(asset);
                }


                totalSum += Math.Round(asset.Count * priceForItem, 2);
            }
            return totalSum;
        }

        // add ITotalWearCoefficientProvider to the params
        public static decimal CalculateResidualPriceForItem(IAssetInfo asset, int count = 1)
        {
            var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate, asset.WriteOffDateTime);
            var indexatedValue = Math.Round(asset.Price * indexationCoefficient, 2);

            var coeffSKZ = asset.TotalWearCoefficient;

            var result = Math.Round(indexatedValue * coeffSKZ, 2) * count;

            return result;
        }
    }
}
