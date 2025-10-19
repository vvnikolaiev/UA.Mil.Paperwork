using Mil.Paperwork.Domain.DataModels.Assets;

namespace Mil.Paperwork.Domain.Calculators
{
    public interface IResidualPriceCalculator
    {
        IList<decimal> GetCoefficients(IAssetInfo asset, DateTime reportDate);

        decimal CalculateTotalWearCoefficient(IAssetInfo asset, DateTime reportDate);
    }
}
