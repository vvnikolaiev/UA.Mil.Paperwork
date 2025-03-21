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

        public static decimal CalculateResidualPriceForItem(AssetInfo asset, DateTime writeOffDate, int count = 1)
        {
            var explotationCoefficient = CoefficientsHelper.GetExploitationCoefficient(asset.StartDate, writeOffDate);
            var workCoefficient = CoefficientsHelper.GetWorkCoefficient(asset.CapacityLeftPercantage);
            var technicalStateCoefficient = CoefficientsHelper.GetTechnicalStateCoefficient(asset.Category);
            var indexationCoefficient = CoefficientsHelper.GetIndexationCoefficient(asset.StartDate, writeOffDate);

            var totalCoefficient = asset.WearAndTearCoeff * explotationCoefficient * workCoefficient * technicalStateCoefficient * indexationCoefficient;

            var result = asset.Price * totalCoefficient * count;

            return result;
        }
    }
}
