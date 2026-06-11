namespace Mil.Paperwork.DataAccess.DataModels.Snapshots
{
    public class AssetValuationSnapshot
    {
        public string Name { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string NomenclatureCode { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime ValuationDate { get; set; }
        public List<AssetComponentSnapshot> AssetComponents { get; set; } = [];
    }
}
