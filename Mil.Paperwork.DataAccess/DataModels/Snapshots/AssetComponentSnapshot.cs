namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class AssetComponentSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string NomenclatureCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Category { get; set; }
        public decimal Price { get; set; }
        public bool Exclude { get; set; }
    }
}
