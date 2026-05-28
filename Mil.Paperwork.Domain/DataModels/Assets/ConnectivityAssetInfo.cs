using Mil.Paperwork.Domain.Calculators;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class ConnectivityAssetInfo : AssetInfo
    {
        public decimal WearAndTearCoeff { get; set; } = 0.8m;

        public override IResidualPriceCalculator GetCalculator()
        {
            var calculator = new ConnectivityResidualPriceCalculator();
            return calculator;
        }
    }
}
