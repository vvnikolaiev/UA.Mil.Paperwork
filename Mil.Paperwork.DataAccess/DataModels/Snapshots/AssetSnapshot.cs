using Mil.Paperwork.DataAccess.Enums;

namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class AssetSnapshot
    {
        public AssetSnapshotKind Kind { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string MeasurementUnit { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string NomenclatureCode { get; set; } = string.Empty;
        public int InitialCategory { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public DateTime StartDate { get; set; }
        public int EventType { get; set; }
        public string TSRegisterNumber { get; set; } = string.Empty;
        public string TSDocumentNumber { get; set; } = string.Empty;
        public int WarrantyPeriodMonths { get; set; }
        public int YearManufactured { get; set; }
        public int ResourceYears { get; set; }
        public bool? IsLocal { get; set; }
        public decimal? WearAndTearCoeff { get; set; }
    }
}
