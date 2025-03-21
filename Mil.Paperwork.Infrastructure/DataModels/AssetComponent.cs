namespace Mil.Paperwork.Infrastructure.DataModels
{
    public class AssetComponent
    {
        public string Name { get; set; }
        public string Unit { get; set; }
        public string NomenclatureCode { get; set; }
        public int Quantity { get; set; }
        public int Category { get; set; } = 2;
        public decimal Price { get; set; }
        public bool Exclude { get; set; }
    }
}
