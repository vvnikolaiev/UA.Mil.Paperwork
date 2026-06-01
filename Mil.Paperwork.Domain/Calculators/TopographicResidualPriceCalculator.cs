using Mil.Paperwork.Domain.DataModels.Assets;
using Mil.Paperwork.Domain.Resources;

namespace Mil.Paperwork.Domain.Calculators
{
    internal class TopographicResidualPriceCalculator : DefaultResidualPriceCalculator
    {
        public override IList<string> GetColumnHeaders() =>
            [ResidualValueReportStrings.CoeffExploitation,
             ResidualValueReportStrings.CoeffChemicalAging,
             ResidualValueReportStrings.CoeffTechState];

        public override IList<decimal> GetCoefficients(IAssetInfo asset, DateTime reportDate) =>
            GetColumnHeaders().Select(_ => 1m).ToList();

        public override decimal CalculateTotalWearCoefficient(IAssetInfo asset, DateTime reportDate) => 1m;
    }
}
