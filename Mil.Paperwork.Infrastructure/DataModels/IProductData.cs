namespace Mil.Paperwork.Infrastructure.DataModels
{
    public interface IProductData
    {
        string Name { get; set; }
        string ShortName { get; set; }
        string MeasurementUnit { get; set; }
        string NomenclatureCode { get; set; }
        int WarrantyPeriodYears { get; set; }

        // shouldn't be here. It simplifies creating reports but leads to make more mistakes
        decimal Price { get; set; }
        DateTime StartDate { get; set; }
    }
}
