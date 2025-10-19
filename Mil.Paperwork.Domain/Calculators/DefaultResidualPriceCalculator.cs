using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.Domain.Calculators
{
    internal class DefaultResidualPriceCalculator : IResidualPriceCalculator
    {
        public virtual IList<decimal> GetCoefficients(IAssetInfo asset, DateTime reportDate)
        {
            return [];
        }

        public virtual decimal CalculateTotalWearCoefficient(IAssetInfo asset, DateTime reportDate)
        {
            return 1;
        }
    }
}
