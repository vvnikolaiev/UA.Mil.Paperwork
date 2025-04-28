using Mil.Paperwork.Domain.Helpers;
using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{

    public interface IAssetInfo
    {
        IAssetValuationData? ValuationData { get; set; }

        string Name { get; set; }
        string ShortName { get; set; }
        string MeasurementUnit { get; set; }
        string SerialNumber { get; set; }
        string NomenclatureCode { get; set; }
        int Category { get; set; }
        decimal Price { get; set; }
        int Count { get; set; }
        DateTime StartDate { get; set; }

        string TSRegisterNumber { get; set; }
        string TSDocumentNumber { get; set; }

        DateTime WriteOffDateTime { get; set; }

        int WarrantyPeriodYears { get; set; }
        decimal TotalWearCoefficient { get; }
    }

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

    }

    public class ConnectivityAssetInfo : AssetInfo
    {
        public decimal WearAndTearCoeff { get; set; } = 0.8m;
        public int CapacityLeftPercantage { get; } = 100;

        public override decimal TotalWearCoefficient => CalculateTotalWearCoefficient();

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
        public bool IsLocal { get; } = true;

        public override decimal TotalWearCoefficient => GetTotalCoefficient();

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
