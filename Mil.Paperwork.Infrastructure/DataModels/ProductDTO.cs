namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string MeasurementUnit { get; set; }
        public string NomenclatureCode { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public int WarrantyPeriodYears { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
