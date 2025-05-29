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

        public static decimal CalculateResidualPriceForItem(IAssetInfo asset, int count = 1)
        {
            var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate, asset.WriteOffDateTime);
            
            var result = CalculateResidualPrice(asset.Price, indexationCoefficient, asset.TotalWearCoefficient, count);

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
