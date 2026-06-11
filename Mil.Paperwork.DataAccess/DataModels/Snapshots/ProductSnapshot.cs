namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class ProductSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string MeasurementUnit { get; set; } = string.Empty;
        public string NomenclatureCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public int WarrantyPeriodMonths { get; set; }
        public int YearManufactured { get; set; }
        public int ResourceYears { get; set; }
    }
}
