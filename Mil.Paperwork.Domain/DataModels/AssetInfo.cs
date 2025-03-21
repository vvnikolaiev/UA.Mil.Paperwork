using Mil.Paperwork.Infrastructure.DataModels;

namespace Mil.Paperwork.Domain.DataModels
{
    public class AssetInfo
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
        public decimal WearAndTearCoeff { get; set; } = 0.8m;
        public int CapacityLeftPercantage { get; } = 100;
        public DateTime StartDate { get; set; } = new DateTime(2023, 01, 01);

        public string TSRegisterNumber { get; set; }
        public string TSDocumentNumber { get; set; }

        public int WarrantyPeriodYears { get; set; } = 1;
    }
}
