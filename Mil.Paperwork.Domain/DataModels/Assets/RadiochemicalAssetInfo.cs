using Mil.Paperwork.Domain.Helpers;

namespace Mil.Paperwork.Domain.DataModels.Assets
{
    public class RadiochemicalAssetInfo : AssetInfo
    {
        // if from USA then different coefficient
        public bool IsLocal { get; set; } = true;

        public override decimal TotalWearCoefficient => GetTotalCoefficient();

        public override IList<decimal> GetCoefficients()
        {
            return new List<decimal>();
        }

        // категорія + сша/наше
        private decimal GetTotalCoefficient()
        {
            var category = ReportHelper.ConvertEventTypeToCategory(InitialCategory, EventType);
            
            decimal coeffSKZ = (category, IsLocal) switch
            {
                (1, true) => 1,
                (1, false) => 1,
                (2, true) => 0.8m,
                (2, false) => 0.8m,
                (3, true) => 0.6m,
                (3, false) => 0.6m,
                (4, true) => 0.4m,
                (4, false) => 0.4m,
                (5, true) => 0.3m,
                (5, false) => 0.2m,
                _ => throw new NotImplementedException()
            };

            return coeffSKZ;
        }
    }
}
