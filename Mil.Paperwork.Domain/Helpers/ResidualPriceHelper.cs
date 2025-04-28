using Mil.Paperwork.Domain.DataModels;

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
                    priceForItem = CalculateResidualPriceForItem(asset, reportData.ReportDate);
                }


                totalSum += Math.Round(asset.Count * priceForItem, 2);
            }
            return totalSum;
        }

        // add ITotalWearCoefficientProvider to the params
        public static decimal CalculateResidualPriceForItem(IAssetInfo asset, DateTime writeOffDate, int count = 1)
        {
            var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate, writeOffDate);
            var indexatedValue = Math.Round(asset.Price * indexationCoefficient, 2);

            var coeffSKZ = asset.TotalWearCoefficient;

            var result = Math.Round(indexatedValue * coeffSKZ, 2) * count;

            return result;
        }
    }
}
