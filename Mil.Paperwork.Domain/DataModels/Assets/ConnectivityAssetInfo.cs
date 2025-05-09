using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class ConnectivityAssetInfo : AssetInfo
    {
        public decimal WearAndTearCoeff { get; set; } = 0.8m;
        public int CapacityLeftPercantage { get; } = 100;

        public override decimal TotalWearCoefficient => CalculateTotalWearCoefficient();

        public override IList<decimal> GetCoefficients()
        {
            var category = ReportHelper.ConvertEventTypeToCategory(InitialCategory, EventType);

            var explotationCoefficient = CoefficientsHelper.GetExploitationCoefficient(StartDate, WriteOffDateTime);
            var workCoefficient = CoefficientsHelper.GetWorkCoefficient(CapacityLeftPercantage);
            var technicalStateCoefficient = CoefficientsHelper.GetTechnicalStateCoefficient(category);

            var coeficients = new List<decimal>() { explotationCoefficient, workCoefficient, WearAndTearCoeff, technicalStateCoefficient };
            return coeficients;
        }

        private decimal CalculateTotalWearCoefficient()
        {
            var category = ReportHelper.ConvertEventTypeToCategory(InitialCategory, EventType);

            var explotationCoefficient = CoefficientsHelper.GetExploitationCoefficient(StartDate, WriteOffDateTime);
            var workCoefficient = CoefficientsHelper.GetWorkCoefficient(CapacityLeftPercantage);
            var technicalStateCoefficient = CoefficientsHelper.GetTechnicalStateCoefficient(category);

            var coeffSKZ = WearAndTearCoeff * explotationCoefficient * workCoefficient * technicalStateCoefficient;

            return coeffSKZ;
        }
    }
}
