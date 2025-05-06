using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public abstract class AssetInfo : IAssetInfo
    {
        public IAssetValuationData? ValuationData { get; set; }

        public string Name { get; set; }
        public string ShortName { get; set; }
        public string MeasurementUnit { get; set; }
        public string SerialNumber { get; set; }
        public string NomenclatureCode { get; set; }
        public int Category { get; set; } = 2;
        public decimal Price { get; set; }
        public int Count { get; set; } = 1;
        public DateTime StartDate { get; set; } = new DateTime(2023, 01, 01);

        public DateTime WriteOffDateTime { get; set; } = new DateTime(2023, 01, 01);
        public string TSRegisterNumber { get; set; }
        public string TSDocumentNumber { get; set; }

        public int WarrantyPeriodYears { get; set; } = 1;

        public abstract decimal TotalWearCoefficient { get; }

        // https://zakon.rada.gov.ua/laws/show/759-98-%D0%BF#n330
        public abstract IList<decimal> GetCoefficients();
    }

    public class ConnectivityAssetInfo : AssetInfo
    {
        public decimal WearAndTearCoeff { get; set; } = 0.8m;
        public int CapacityLeftPercantage { get; } = 100;

        public override decimal TotalWearCoefficient => CalculateTotalWearCoefficient();

        public override IList<decimal> GetCoefficients()
        {
            var explotationCoefficient = CoefficientsHelper.GetExploitationCoefficient(StartDate, WriteOffDateTime);
            var workCoefficient = CoefficientsHelper.GetWorkCoefficient(CapacityLeftPercantage);
            var technicalStateCoefficient = CoefficientsHelper.GetTechnicalStateCoefficient(Category);

            var coeficients = new List<decimal>() { explotationCoefficient, workCoefficient, WearAndTearCoeff, technicalStateCoefficient };
            return coeficients;
        }

        private decimal CalculateTotalWearCoefficient()
        {
            var explotationCoefficient = CoefficientsHelper.GetExploitationCoefficient(StartDate, WriteOffDateTime);
            var workCoefficient = CoefficientsHelper.GetWorkCoefficient(CapacityLeftPercantage);
            var technicalStateCoefficient = CoefficientsHelper.GetTechnicalStateCoefficient(Category);

            var coeffSKZ = WearAndTearCoeff * explotationCoefficient * workCoefficient * technicalStateCoefficient;

            return coeffSKZ;
        }
    }

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
            decimal coeffSKZ = (Category, IsLocal) switch
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
