using Mil.Paperwork.Domain.Calculators;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class RadiochemicalAssetInfo : AssetInfo
    {
        // if from USA then different coefficient
        public bool IsLocal { get; set; } = true;

        public override IResidualPriceCalculator GetCalculator()
        {
            var calculator = new RadiochemicalResidualPriceCalculator();
            return calculator;
        }
    }
}